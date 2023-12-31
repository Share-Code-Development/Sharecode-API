namespace Sharecode.Backend.Application.Features.Http.Snippet.Reactions.Get;

public class GetReactionsOfSnippetByUserResponse
{
    public HashSet<string> Reactions = [];

    private GetReactionsOfSnippetByUserResponse(HashSet<string> reactions)
    {
        Reactions = reactions;
    }

    public static GetReactionsOfSnippetByUserResponse From(HashSet<string> reactions)
    {
        return new GetReactionsOfSnippetByUserResponse(reactions);
    }

    public static GetReactionsOfSnippetByUserResponse Empty => new([]);
}