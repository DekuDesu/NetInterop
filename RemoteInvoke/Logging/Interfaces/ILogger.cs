namespace RemoteInvoke.Logging
{
    public interface ILogger
    {
        void Error(string value);
        void Information(string value);
        void Log(string value);
        void LogLine(string value);
        void Warning(string value);
    }
}