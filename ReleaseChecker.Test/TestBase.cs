using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace PanoramicData.SecurityChecker.Test;

public class TestBase
{
    public TestBase(ITestOutputHelper testOutputHelper)
    {
        Logger = testOutputHelper.BuildLogger();
    }

    public readonly ILogger Logger;
}
