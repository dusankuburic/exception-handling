using Serilog;

namespace Tegla.Persistence.Brokers.Loggings;

public partial class LoggingBroker : ILoggingBroker
{
    private readonly ILogger _logger;

    public LoggingBroker(ILogger logger) =>
        _logger = logger;

    public void LogCritical(Exception exception) =>
        _logger.Fatal(exception, exception.Message);

    public void LogDebug(string message) =>
        _logger.Debug(message);

    public void LogError(Exception exception) =>
        _logger.Error(exception, exception.Message);

    public void LogInformation(string message) =>
        _logger.Information(message);

    public void LogTrace(string message) =>
        _logger.Verbose(message);

    public void LogWarning(string message) =>
        _logger.Warning(message);
}
