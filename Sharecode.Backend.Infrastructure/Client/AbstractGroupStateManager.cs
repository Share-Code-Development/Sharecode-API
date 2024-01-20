using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Infrastructure.Client;

public abstract class AbstractGroupStateManager(
    IOptions<LiveGroupStateManagementConfiguration> configuration,
    IServerSession serverSession)
    : IGroupStateManager
{
    private readonly LiveGroupStateManagementConfiguration _configuration = configuration.Value;

    public string SessionValue => serverSession.Current.ToString();
    
    public abstract void OnAppInit(IServiceScope executionScope);

    public abstract void OnAppDestruct(IServiceScope executionScope);

    public abstract Task<bool> AddAsync(string groupName, string connectionId, string userIdentifier, CancellationToken token = default);
    public abstract Task<bool> RemoveAsync(string groupName, string connectionId, CancellationToken token = default);
    public abstract Task<bool> IsMemberAsync(string groupName, string connectionId, CancellationToken token = default);
    public abstract Task<Dictionary<string, string>> Members(string groupNam, CancellationToken token = default);
    public abstract Task<HashSet<string>> GetAllGroupsAsync(string connectionId, CancellationToken token = default);
}