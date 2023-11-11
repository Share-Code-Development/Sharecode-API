using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Entity;

namespace Sharecode.Backend.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    void Register(User user);
}