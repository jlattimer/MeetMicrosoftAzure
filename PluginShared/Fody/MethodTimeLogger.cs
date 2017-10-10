using System.Reflection;

internal static class MethodTimeLogger
{
    public static AppInsights AiLogger;

    public static void Log(MethodBase methodBase, long milliseconds)
    {
        string operationName = LogHelper.GetFullMethodName(methodBase);
        AiLogger.WriteMethodPerfMetric(operationName, milliseconds, operationName);
    }
}