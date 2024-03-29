﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using StackExchange.Redis;

namespace Sharecode.Backend.Infrastructure.Client;

public class RedisGroupStateManager([FromKeyedServices(DiKeyedServiceConstants.RedisForSignalRStateManagement)]ConfigurationOptions connectionConfiguration, IOptions<LiveGroupStateManagementConfiguration> groupStateConfiguration, IServerSession serverSession, ILogger logger) : AbstractGroupStateManager(groupStateConfiguration, serverSession)
{
    private ConnectionMultiplexer? Redis { get; set; }
    private static readonly object Lock = new();
    private readonly int _maxRetryCount = 3;
    private readonly TimeSpan _initialRetryDelay = TimeSpan.FromSeconds(2);

    public override void OnAppInit(IServiceScope executionScope)
    {
        long cursor = 0;
        do
        {
            RedisResult scanResult = Database.Execute("SCAN", cursor.ToString(), "MATCH", $"{serverSession.Current.ServerId}-*");
            var innerResult = (RedisResult[])scanResult;
            cursor = long.Parse((string)innerResult[0]);

            RedisKey[] keysToDelete = (RedisKey[])innerResult[1];
            foreach (var key in keysToDelete)
            {
                Database.KeyDelete(key);
            }
        } while (cursor != 0);
    }

    public override void OnAppDestruct(IServiceScope executionScope)
    {
        long cursor = 0;
        do
        {
            RedisResult scanResult = Database.Execute("SCAN", cursor.ToString(), "MATCH", $"{SessionValue}-*");
            var innerResult = (RedisResult[])scanResult;
            cursor = long.Parse((string)innerResult[0]);

            RedisKey[] keysToDelete = (RedisKey[])innerResult[1];
            foreach (var key in keysToDelete)
            {
                Database.KeyDelete(key);
            }
        } while (cursor != 0);
    }

    public override async Task<bool> AddAsync(string groupName, string connectionId, string userIdentifier, CancellationToken token = default)
    {
        try
        {
            #region Mapping User Connections to Group

            var key = new RedisKey($"{SessionValue}:{groupName}");
            HashEntry[] entries = new[] { new HashEntry(connectionId, userIdentifier) };
            await Database.HashSetAsync(key, entries);

            #endregion
            #region Adding users group to the connectionId

            RedisKey connectionKey = new RedisKey($"{SessionValue}:{connectionId}");
            RedisValue value = new RedisValue(groupName);
            await Database.SetAddAsync(connectionKey, value);

            #endregion
            return true;
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to add the user with {ConnectionId} - {Identifier} to the group {Group} due to {Message}", connectionId, userIdentifier, groupName, e.Message);
            return false;
        }
    }

    public override async Task<bool> RemoveAsync(string groupName, string connectionId, CancellationToken token = default)
    {
        try
        {
            var key = new RedisKey($"{SessionValue}:{groupName}");
            return await Database.HashDeleteAsync(key, new RedisValue(connectionId));
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to delete the user with {ConnectionId}  to the group {Group} due to {Message}", connectionId, groupName, e.Message);
            return false;
        }
    }

    public override async Task<bool> IsMemberAsync(string groupName, string connectionId, CancellationToken token = default)
    {
        try
        {
            var key = new RedisKey($"{SessionValue}:{groupName}");
            var entries = Database.HashScanAsync(key, new RedisValue(connectionId));
            await foreach (var entry in entries.WithCancellation(token)) // Don't forget to use the CancellationToken
            {
                if(entry.Value.ToString() == connectionId)
                    return true;
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to scan the group {Group} for connection id {Id} due to {Message}", groupName, connectionId, e.Message);
        }
        return false;
    }

    public override async Task<Dictionary<string, string>> Members(string groupName, CancellationToken token = default)
    {
        try
        {
            var key = new RedisKey($"{SessionValue}:{groupName}");
            var entries = await Database.HashGetAllAsync(key);
            
            return entries.ToDictionary(
                entry => entry.Name.ToString(), 
                entry => entry.Value.ToString());
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to get members of the group {GroupName} due to {Message}", groupName, e.Message);
            return new Dictionary<string, string>();
        }
    }

    public override async Task<HashSet<string>> GetAllGroupsAsync(string connectionId, CancellationToken token = default)
    {
        HashSet<string> groups = [];
        try
        {
            RedisKey key = new RedisKey($"{SessionValue}:{connectionId}");
            var members = await Database.SetMembersAsync(key);
            foreach (var member in members)
            {
                groups.Add(member.ToString());
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to get all the existing groups of the connection {Connection} due to {Error}", connectionId, e.Message);
        }

        return groups;
    }

    private IDatabase Database
    {
        get
        {
            if (Redis is { IsConnected: true })
            {
                return Redis.GetDatabase();
            }

            lock (Lock)
            {
                //Double checking whether the connection got reestablished or not
                if (Redis is { IsConnected: true })
                {
                    return Redis.GetDatabase();
                }

                for (int retry = 0; retry < _maxRetryCount; retry++)
                {
                    try
                    {
                        Redis?.Dispose();
                        Redis = ConnectionMultiplexer.Connect(connectionConfiguration);
                        return Redis.GetDatabase();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Failed to reconnect to Redis-[{Type}]. Attempt {AttemptNumber}", retry + 1, DiKeyedServiceConstants.RedisForSignalRStateManagement);
                        Thread.Sleep(_initialRetryDelay * (int)Math.Pow(2, retry));
                    }
                }

                throw new ApplicationException($"Failed to reconnect to Redis-[{DiKeyedServiceConstants.RedisForSignalRStateManagement}] after multiple attempts.");
            }
        }
    }
}