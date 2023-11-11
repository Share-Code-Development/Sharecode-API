using Sharecode.Backend.Domain.Base;

namespace Sharecode.Backend.Application.Data;

public interface IUnitOfWork : IDisposable
{
    void Commit();
    Task CommitAsync();    
}