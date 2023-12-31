using MediatR;

namespace Sharecode.Backend.Application.Features.Http.Snippet.UpdateStats;

public class UpdateStatisticsQueryHandler : IRequestHandler<UpdateStatisticsCommand, UpdateStatisticsResponse>
{
    public async Task<UpdateStatisticsResponse> Handle(UpdateStatisticsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}