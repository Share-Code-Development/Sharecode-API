using Sharecode.Backend.Application.Features.Http.Refresh.Get;

namespace Sharecode.Backend.IntegrationTest.Test.Cases;

public class SampleTestCase(IntegrationTestFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Test()
    {
        GetRefreshTokenCommand command = new GetRefreshTokenCommand("asdad8iasdoijasoida");
        var response = await Sender.Send(command);
        Assert.Null(response);
    }
}