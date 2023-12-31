using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application.Features.Http.Users.GetMySnippets;

public class GetMySnippetsQuery : ListQuery, IAppRequest<GetMySnippetsResponse>
{
    public bool OnlyOwned { get; set; } = true;
    public bool RecentSnippets { get; set; } = false;
    public override string DefaultOrderBy()
    {
        return string.Empty;
    }
}