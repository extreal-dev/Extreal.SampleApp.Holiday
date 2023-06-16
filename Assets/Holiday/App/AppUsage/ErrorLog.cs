using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class ErrorLog
    {
        public string LogString { get; }
        public string StackTrace { get; }
        public LogType LogType { get; }

        public ErrorLog(string logString, string stackTrace, LogType type)
        {
            LogString = logString;
            StackTrace = stackTrace;
            LogType = type;
        }
    }
}
