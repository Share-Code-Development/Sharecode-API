using MediatR;
using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Users.ForgotPassword;

public record ForgotPasswordCommand(string EmailAddress) : IAppRequest<bool>;