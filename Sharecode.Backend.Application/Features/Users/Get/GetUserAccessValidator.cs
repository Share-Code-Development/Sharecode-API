using FluentValidation;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Features.Users.Get;

public class GetUserByIdAccessValidator : AbstractValidator<GetUserByIdQuery>
{
    private readonly IHttpClientContext _context;

    public GetUserByIdAccessValidator(IHttpClientContext context)
    {
        _context = context;
        /*RuleFor(x => x.UserId)
            .MustAsync( async (userId, token) =>
            {
                var requestingUser = await context.GetUserIdentifierAsync();
                if (!requestingUser.HasValue)
                    return false;

                var user = requestingUser.Value;
                //TODO Check role later
                if (user != userId)
                {
                    return await _context.HasPermissionAsync(Permissions.AccessProfileOthers, token);
                }
                return true;
            })
            .WithErrorCode("10920")
            .WithMessage("Invalid access");*/
    }
}

public class GetUserByEmailAccessValidator : AbstractValidator<GetUserByEmailQuery>
{
    private readonly IHttpClientContext _context;

    public GetUserByEmailAccessValidator(IHttpClientContext context)
    {
        _context = context;
        /*RuleFor(x => x.EmailAddress)
            .MustAsync( async (userId, token) =>
            {
                var requestingUser = context.EmailAddress;
                if (string.IsNullOrEmpty(requestingUser))
                    return false;

                var user = requestingUser;
                //TODO Check role later
                if (user != userId)
                {
                    return await _context.HasPermissionAsync(Permissions.AccessProfileOthers, token);
                }
                return true;
            })
            .WithErrorCode("10920")
            .WithMessage("Invalid access");*/
    }
}