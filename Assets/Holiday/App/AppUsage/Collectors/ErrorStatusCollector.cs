using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage.Collectors
{
    public class ErrorStatusCollector : IAppUsageCollector
    {
        private readonly AppUsageConfig appUsageConfig;
        private readonly AppUsageEmitter appUsageEmitter;

        public ErrorStatusCollector(AppUsageConfig appUsageConfig, AppUsageEmitter appUsageEmitter)
        {
            this.appUsageConfig = appUsageConfig;
            this.appUsageEmitter = appUsageEmitter;
        }

        public IDisposable Collect(Action<AppUsageBase> collect) =>
            appUsageEmitter.OnErrorOccurred
                .Where(errorLog => errorLog.LogType is LogType.Error or LogType.Exception)
                .Hook(errorLog => collect?.Invoke(
                    ErrorStatus.Of(
                        errorLog.LogString,
                        null,
                        errorLog.StackTrace,
                        errorLog.LogType,
                        appUsageConfig)));

        [SuppressMessage("Usage", "IDE1006")]
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
                    stackTrace = $"{stackTrace[..appUsageConfig.MaxStackTraceLength]}...";
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
}
