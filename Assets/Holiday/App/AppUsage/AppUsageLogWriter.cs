using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine;
using UnityEngine.Networking;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class AppUsageLogWriter : ILogWriter
    {
        private readonly AppUsageConfig appUsageConfig;
        private readonly AppScope.AppStateProvider appStateProvider;
        private readonly ILogWriter defaultLogWriter;

        public AppUsageLogWriter(
            AppUsageConfig appUsageConfig,
            AppScope.AppStateProvider appStateProvider,
            ILogWriter defaultLogWriter = null)
        {
            this.appUsageConfig = appUsageConfig;
            this.appStateProvider = appStateProvider;
            this.defaultLogWriter = defaultLogWriter ?? new UnityDebugLogWriter();
        }

        private const string AppUsageCategory = nameof(AppUsage);

        public void Log(LogLevel logLevel, string logCategory, string message, Exception exception = null)
        {
            if (!appUsageConfig.Enable)
            {
                defaultLogWriter.Log(logLevel, logCategory, message, exception);
                return;
            }

            if (AppUsageCategory == logCategory)
            {
                SendAppUsage(message);
            }
            else if (LogLevel.Error == logLevel)
            {
                SendErrorLog(message, exception);
            }
            else
            {
                defaultLogWriter.Log(logLevel, logCategory, message, exception);
            }
        }

        private void SendAppUsage(string jsonLogLine) => PostAsync(jsonLogLine).Forget();

        [SuppressMessage("Usage", "CC0022"), SuppressMessage("Design", "CC0004"), SuppressMessage("Usage", "RS0030")]
        private async UniTask PostAsync(string jsonLogLine)
        {
            var statusCode = default(long);
            var exception = default(Exception);
            try
            {
                var body = ToPostBody(jsonLogLine);
                using var request = new UnityWebRequest(appUsageConfig.PushUrl, UnityWebRequest.kHttpVerbPOST)
                {
                    uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)),
                    downloadHandler = new DownloadHandlerBuffer(),
                    timeout = appUsageConfig.TimeoutSeconds
                };
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Scope-OrgID", appUsageConfig.OrgId);
                await request.SendWebRequest();
                statusCode = request.responseCode;
            }
            catch (Exception e)
            {
                exception = e;
            }
#if !HOLIDAY_PROD
            // Logging for debugging during development.
            // ELogger cannot be used because of the infinite loop.
            if (statusCode != 0)
            {
                Debug.Log($"{nameof(AppUsageLogWriter)}:{statusCode}");
                Debug.Log(jsonLogLine);
            }

            if (exception != null)
            {
                Debug.Log(exception.Message + Environment.NewLine + exception.StackTrace);
            }
#endif
        }

        private static readonly string JsonPostBody =
            "{\"streams\":[{\"stream\":{\"app\":\"@value@\"},\"values\":[[\"@epochNS@\",\"@logLine@\"]]}]}";

        private static string ToPostBody(string jsonLogLine) =>
            JsonPostBody
                .Replace("@value@", nameof(Holiday))
                .Replace("@epochNS@", (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000000).ToString())
                .Replace("@logLine@", jsonLogLine.Replace("\"", "\\\""));

        private void SendErrorLog(string message, Exception exception = null)
        {
            var errorStatus = ErrorStatus.Of(message, exception?.StackTrace, LogType.Error, appUsageConfig);
            SendAppUsage(AppUsageUtils.ToJson(errorStatus, appUsageConfig, appStateProvider.AppState));
        }
    }
}
