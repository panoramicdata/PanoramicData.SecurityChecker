using Microsoft.Extensions.Logging;

namespace PanoramicData.SecurityChecker.Test.Fakes;

internal class TestLogger<T> : ILogger<T> where T : class
{
    private readonly ILogger _logger;

    public TestLogger(ILogger logger)
    {
        _logger = logger;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
