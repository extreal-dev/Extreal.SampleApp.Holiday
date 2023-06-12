﻿using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class ErrorStatus : AppUsageBase
    {
        public string ErrorType;
        public string ErrorMessage;
        public string ExceptionMessage;
        public string StackTrace;

        public static ErrorStatus Of(string logString, string exceptionMessage, string stackTrace, LogType type, AppUsageConfig appUsageConfig)
        {
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Length > appUsageConfig.MaxStackTraceLength)
            {
                stackTrace = $"{stackTrace.Substring(0, appUsageConfig.MaxStackTraceLength)}...";
            }
            return new ErrorStatus
            {
                UsageId = nameof(ErrorStatus),
                ErrorMessage = logString,
                ExceptionMessage = exceptionMessage,
                StackTrace = stackTrace,
                ErrorType = type.ToString()
            };
        }
    }
}
