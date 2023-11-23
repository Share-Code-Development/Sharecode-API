using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Outbox;

namespace Sharecode.Backend.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class ProcessOutboxJob(ShareCodeDbContext dbContext, IPublisher publisher, ILogger<ProcessOutboxJob> logger,
        IUnitOfWork unitOfWork)
    : IJob
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> messages = await FetchMessages(context.CancellationToken);
        foreach (OutboxMessage outboxMessage in messages)
        {
            List<string> errors = null;
            IDomainEvent? domainEvent = null;
            try
            {
                outboxMessage.Attempt++;
                domainEvent = JsonConvert
                    .DeserializeObject<IDomainEvent>(outboxMessage.Content, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
            }
            catch (Exception e)
            {
                string errorMessage = $"Failed to deserialize outbox message, The deserializer has thrown the error." +
                                      $"MessageType : {outboxMessage.Type}, " +
                                      $"MessageContent : {outboxMessage.Content}, " +
                                      $"Attempt: {outboxMessage.Attempt}. " +
                                      $"Error Message: {e.Message}, " +
                                      $"Error Stacktrace: {e.StackTrace}";
                logger.LogCritical(errorMessage);
                errors ??= JsonConvert.DeserializeObject<List<string>>(outboxMessage.Error) ?? new List<string>();
                errors.Add(errorMessage);
                outboxMessage.Error = JsonConvert.SerializeObject(errors);
                continue;   
            }

            
            if (domainEvent == null)
            {
                errors ??= JsonConvert.DeserializeObject<List<string>>(outboxMessage.Error) ?? new List<string>();
                string errorMessage = $"Failed to deserialize outbox message. " +
                                      $"MessageType : {outboxMessage.Type}, " +
                                      $"MessageContent : {outboxMessage.Content}, " +
                                      $"Attempt: {outboxMessage.Attempt}. ";
                logger.LogCritical(errorMessage);
                errors.Add(errorMessage);
                outboxMessage.Error = JsonConvert.SerializeObject(errors);
                continue;   
            }
            
            try
            {
                await publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception e)
            {
                string errorMessage = $"Failed to publish an event to the handler." +
                                      $"MessageType : {outboxMessage.Type}, " +
                                      $"MessageContent : {outboxMessage.Content}, " +
                                      $"Attempt: {outboxMessage.Attempt}. " +
                                      $"Error Message: {e.Message}, " +
                                      $"Error Stacktrace: {e.StackTrace}";
                logger.LogCritical(errorMessage);
                errors ??= JsonConvert.DeserializeObject<List<string>>(outboxMessage.Error) ?? new List<string>();
                errors.Add(errorMessage);
                outboxMessage.Error = JsonConvert.SerializeObject(errors);
                continue;
            }

            outboxMessage.Error = errors == null ? "[]" : JsonConvert.SerializeObject(errors);
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        await _unitOfWork.CommitAsync();
    }

    private async Task<List<OutboxMessage>> FetchMessages(CancellationToken token = default)
    {
        var messages = await dbContext
            .Set<OutboxMessage>()
            .Where(message => message.ProcessedOnUtc == null && message.Attempt <= 3)
            .Take(20)
            .ToListAsync(token);

        return messages;
    }
}