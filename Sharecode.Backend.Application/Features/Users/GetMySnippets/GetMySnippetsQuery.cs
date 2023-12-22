using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application.Features.Users.GetMySnippets;

public class GetMySnippetsQuery : ListQuery
{
    public bool OnlyOwned { get; set; } = true;
}