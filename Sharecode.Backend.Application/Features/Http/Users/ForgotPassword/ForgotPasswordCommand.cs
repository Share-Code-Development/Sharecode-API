using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.ForgotPassword;

public record ForgotPasswordCommand(string EmailAddress) : IAppRequest<bool>;