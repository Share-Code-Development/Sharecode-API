using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Sharecode.Backend.IntegrationTest.Test;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    
    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>(); 
    }
}