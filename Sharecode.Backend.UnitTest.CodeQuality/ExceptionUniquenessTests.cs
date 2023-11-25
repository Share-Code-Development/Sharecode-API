using FluentAssertions;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.ExceptionDetail;
using Xunit;
using Xunit.Abstractions;

namespace Sharecode.Backend.UnitTest.CodeQuality;

public class ExceptionUniquenessTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ExceptionUniquenessTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void AllExceptionTypesShouldHaveUniqueNamesAndErrorCodes()
    {
        var exceptionDetailClient = ExceptionDetailClient.FromAssemblies(Sharecode.Backend.Api.Sharecode.ReferencingAssemblies).CollectErrors(typeof(AppException));
        
        var allExceptions = exceptionDetailClient.Errors;
        
        _testOutputHelper.WriteLine(allExceptions.Count.ToString());

        allExceptions.GroupBy(e => e.Value.ErrorCode)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .Should().BeEmpty("because no two exceptions should have the same error code");

        allExceptions.GroupBy(e => e.Value.NormalizedClassName)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .Should().BeEmpty("because no two exceptions should have the same class name");
    }
}