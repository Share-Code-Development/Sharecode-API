namespace Sharecode.Backend.Application.Service;

public interface IUserService
{
    Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default);
}