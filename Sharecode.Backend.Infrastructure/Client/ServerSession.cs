using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Models;

namespace Sharecode.Backend.Infrastructure.Client;

public class ServerSession : IServerSession
{

    private static ServerSession _instance = null!;

    public ServerSession(string id)
    {
        if (_instance != null)
            throw new ApplicationException("Please do not try to create a server session yourself. Please use DI to manage ServerSession");

        _instance = this;
        var env = Environment.GetEnvironmentVariable("SHARECODE_BACKEND_SERVER_ID");
        if (string.IsNullOrEmpty(env))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ApplicationException("No ServerId or SHARECODE_BACKEND_SERVER_ID is specified");
            }

            env = id;
        }
        Current = Session.Create(env);
    }
    
    public Session Current { get; set; }
}