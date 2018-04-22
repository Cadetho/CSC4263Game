using System.Diagnostics;

public static class Logger
{
    [Conditional("ENABLE_LOGS")]
    public static void Debug(string logMsg)
    {
        UnityEngine.Debug.Log(logMsg);
    }
}
