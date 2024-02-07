namespace Tegla.Persistence.Brokers.Loggings;

public interface ILoggingBroker
{
    void LogInformation(string message);
    void LogDebug(string message);
    void LogTrace(string message);
    void LogWarning(string message);
    void LogError(Exception exception);
    void LogCritical(Exception exception);
}
