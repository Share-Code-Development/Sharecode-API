using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Snippet.UpdateStats;

public class UpdateStatisticsCommand : IAppRequest<UpdateStatisticsResponse>
{
    public bool IncrementView { get; set; }
    public bool IncrementCopy { get; set; }
    public bool UpdateRecent { get; set; }
}