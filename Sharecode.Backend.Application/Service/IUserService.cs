namespace Sharecode.Backend.Application.Service;

public interface IUserService
{
    Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default);
    Task<bool> VerifyUserAsync(Guid userId, CancellationToken token = default);
    Task<bool> RequestForgotPassword(string emailAddress, CancellationToken token = default);
}