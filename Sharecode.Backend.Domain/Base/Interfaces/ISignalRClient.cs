using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Base.Interfaces;

public interface ISignalRClient
{
    Task Message(LiveEvent<object> obj);
}