using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application.Features.Users.TagSearch;

public class SearchUsersForTagCommand : ListQuery, IAppRequest<SearchUserForTagResponse>
{
    public bool ShouldEnableTagging { get; set; } = false;
    
    public override string DefaultOrderBy()
    {
        return string.Empty;
    }
}