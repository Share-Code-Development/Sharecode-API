using Sharecode.Backend.Application.Models;

namespace Sharecode.Backend.Application.Client;

public interface IServerSession
{
    public Session Current { get; protected set; }
}