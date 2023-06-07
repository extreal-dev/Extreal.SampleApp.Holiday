using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class AppUsageManager : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppUsage));

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AppState appState;
        private readonly AppUsageConfig appUsageConfig;

        private IObservable<string> OnFirstUsed => onFirstUsed.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onFirstUsed = new Subject<string>();

        private IObservable<Unit> OnApplicationExiting => onApplicationExiting.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onApplicationExiting = new Subject<Unit>();

        private IObservable<ErrorLog> OnErrorOccured => onErrorOccured.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<ErrorLog> onErrorOccured = new Subject<ErrorLog>();

        public AppUsageManager(StageNavigator<StageName, SceneName> stageNavigator, AppState appState,
            AppUsageConfig appUsageConfig)
        {
            this.stageNavigator = stageNavigator;
            this.appState = appState;
            this.appUsageConfig = appUsageConfig;
        }

        protected override void ReleaseUnmanagedResources()
        {
            disposables.Dispose();

            if (!appUsageConfig.Enable)
            {
                return;
            }

            Application.wantsToQuit -= WantsToQuit;
            Application.logMessageReceived -= LogMessageReceived;
        }

        public void CollectAppUsage()
        {
            if (!appUsageConfig.Enable)
            {
                return;
            }

            Application.wantsToQuit += WantsToQuit;
            Application.logMessageReceived += LogMessageReceived;

            CollectFirstUse();
            CollectStageUsage(stageNavigator, appState);
            CollectResourceUsage();
            CollectErrorStatus();
        }

        private bool WantsToQuit()
        {
            onApplicationExiting.OnNext(Unit.Default);
            return true;
        }

        private void LogMessageReceived(string logString, string stackTrace, LogType type)
            => onErrorOccured.OnNext(new ErrorLog(logString, stackTrace, type));

        private class ErrorLog
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

        private void Collect(AppUsageBase appUsageBase, string clientId = null)
            => Logger.LogInfo(AppUsageUtils.ToJson(appUsageBase, clientId ?? GetClientId(), appState));

        private string GetClientId()
        {
            var clientId = AppUsageUtils.GetClientId(appUsageConfig);
            if (clientId.IsGenerated)
            {
                onFirstUsed.OnNext(clientId.Value);
            }

            return clientId.Value;
        }

        private void CollectFirstUse() =>
            OnFirstUsed
                .Hook(clientId =>
                {
                    Collect(
                        new FirstUse
                        {
                            UsageId = nameof(FirstUse),
                            OS = SystemInfo.operatingSystem,
                            DeviceModel = SystemInfo.deviceModel,
                            DeviceType = SystemInfo.deviceType.ToString(),
                            DeviceId = SystemInfo.deviceUniqueIdentifier,
                            ProcessorType = SystemInfo.processorType
                        },
                        clientId);
                })
                .AddTo(disposables);

        private void CollectStageUsage(StageNavigator<StageName, SceneName> stageNavigator, AppState appState)
        {
            Action collect = () =>
            {
                if (appState.StageState == null)
                {
                    return;
                }
                Collect(new StageUsage
                {
                    UsageId = nameof(StageUsage),
                    StayTimeSeconds = appState.StageState.StayTimeSeconds,
                    NumberOfTextChatsSent = appState.StageState.NumberOfTextChatsSent
                });
            };

            stageNavigator.OnStageTransitioning
                .Hook(_ => collect())
                .AddTo(disposables);

            OnApplicationExiting
                .Hook(_ => collect())
                .AddTo(disposables);
        }

        private void CollectResourceUsage() =>
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(appUsageConfig.ResourceUsageCollectPeriodSeconds))
                .Hook(_ => Collect(new ResourceUsage
                {
                    UsageId = nameof(ResourceUsage),
                    TotalReservedMemoryMb = AppUtils.ToMb(Profiler.GetTotalReservedMemoryLong()),
                    TotalAllocatedMemoryMb = AppUtils.ToMb(Profiler.GetTotalAllocatedMemoryLong()),
                    MonoHeapSizeMb = AppUtils.ToMb(Profiler.GetMonoHeapSizeLong()),
                    MonoUsedSizeMb = AppUtils.ToMb(Profiler.GetMonoUsedSizeLong())
                }))
                .AddTo(disposables);

        private void CollectErrorStatus() =>
            OnErrorOccured
                .Hook(errorLog =>
                {
                    if (errorLog.LogType != LogType.Error && errorLog.LogType != LogType.Exception)
                    {
                        return;
                    }

                    Collect(ErrorStatus.Of(errorLog.LogString, errorLog.StackTrace, errorLog.LogType, appUsageConfig));
                })
                .AddTo(disposables);
    }
}
