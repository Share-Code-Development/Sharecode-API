using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Usage;

public class GetUserSnippetUsageQuery : IAppRequest<GetUserSnippetUsageResponse>
{
    public Guid UserId { get; set; }
}