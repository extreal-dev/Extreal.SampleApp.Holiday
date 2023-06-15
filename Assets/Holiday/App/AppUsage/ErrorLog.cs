using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    [SuppressMessage("Usage", "IDE1006")]
    public class ErrorLog
    {
        public readonly string LogString;
        public readonly string StackTrace;
        public readonly LogType LogType;

        public ErrorLog(string logString, string stackTrace, LogType type)
        {
            LogString = logString;
            StackTrace = stackTrace;
            LogType = type;
        }
    }
}
