using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class AppUsageManager : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppUsage));

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly ICollection<IAppUsageCollector> appUsageCollectors;

        public StageNavigator<StageName, SceneName> StageNavigator { get; }
        public AppState AppState { get; }
        public AppUsageConfig AppUsageConfig { get; }

        public IObservable<string> OnFirstUsed => onFirstUsed.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onFirstUsed = new Subject<string>();

        public IObservable<Unit> OnApplicationExiting => onApplicationExiting.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onApplicationExiting = new Subject<Unit>();

        public IObservable<ErrorLog> OnErrorOccured => onErrorOccured.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<ErrorLog> onErrorOccured = new Subject<ErrorLog>();

        public AppUsageManager(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AppUsageConfig appUsageConfig,
            ICollection<IAppUsageCollector> appUsageCollectors)
        {
            StageNavigator = stageNavigator;
            AppState = appState;
            AppUsageConfig = appUsageConfig;
            this.appUsageCollectors = appUsageCollectors;
        }

        protected override void ReleaseUnmanagedResources()
        {
            disposables.Dispose();

            if (!AppUsageConfig.Enable)
            {
                return;
            }

            Application.wantsToQuit -= WantsToQuit;
            Application.logMessageReceived -= LogMessageReceived;
        }

        public void CollectAppUsage()
        {
            if (!AppUsageConfig.Enable)
            {
                return;
            }

            Application.wantsToQuit += WantsToQuit;
            Application.logMessageReceived += LogMessageReceived;

            foreach (var appUsageCollector in appUsageCollectors)
            {
                disposables.Add(appUsageCollector.Collect(this));
            }
        }

        private bool WantsToQuit()
        {
            onApplicationExiting.OnNext(Unit.Default);
            return true;
        }

        private void LogMessageReceived(string logString, string stackTrace, LogType type)
            => onErrorOccured.OnNext(new ErrorLog(logString, stackTrace, type));

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

        public void Collect(AppUsageBase appUsageBase, string clientId = null)
            => Logger.LogInfo(AppUsageUtils.ToJson(appUsageBase, clientId ?? GetClientId(), AppState));

        private string GetClientId()
        {
            var clientId = AppUsageUtils.GetClientId(AppUsageConfig);
            if (clientId.IsGenerated)
            {
                onFirstUsed.OnNext(clientId.Value);
            }

            return clientId.Value;
        }
    }
}
