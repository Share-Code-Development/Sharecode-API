using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    void Register(User user);
    Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default);
}