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

        public AppUsageManager(StageNavigator<StageName, SceneName> stageNavigator, AppState appState, AppUsageConfig appUsageConfig)
        {
            this.stageNavigator = stageNavigator;
            this.appState = appState;
            this.appUsageConfig = appUsageConfig;
        }

        protected override void ReleaseUnmanagedResources()
        {
            Application.logMessageReceived -= CollectErrorStatus;
            disposables.Dispose();
            Application.wantsToQuit -= WantsToQuit;
        }

        public void CollectAppUsage()
        {
            CollectStageUsage(stageNavigator, appState);
            CollectResourceUsage();
            Application.logMessageReceived += CollectErrorStatus;
        }

        private void CollectStageUsage(StageNavigator<StageName, SceneName> stageNavigator, AppState appState)
        {
            stageNavigator.OnStageTransitioned
                .Hook(appState.SetStage)
                .AddTo(disposables);

            stageNavigator.OnStageTransitioning
                .Hook(_ => SendStageUsage())
                .AddTo(disposables);

            Application.wantsToQuit += WantsToQuit;
        }

        private void SendStageUsage()
        {
            if (appState.StageState == null)
            {
                return;
            }
            Send(appState.StageState.ToStageUsage());
        }

        private bool WantsToQuit()
        {
            SendStageUsage();
            return true;
        }

        private void Send(AppUsageBase appUsageBase) => Send(GetClientId(), appUsageBase);

        private void Send(string clientId, AppUsageBase appUsageBase)
        {
            appUsageBase.ClientId = clientId;
            appUsageBase.StageName = appState.StageState?.StageName.ToString();
            Logger.LogInfo(JsonUtility.ToJson(appUsageBase));
        }

        private string GetClientId()
        {
            if (!PlayerPrefs.HasKey(appUsageConfig.ClientIdKey))
            {
                var clientId = Guid.NewGuid().ToString();
                CollectFirstUse(clientId);
                PlayerPrefs.SetString(appUsageConfig.ClientIdKey, clientId);
            }
            return PlayerPrefs.GetString(appUsageConfig.ClientIdKey);
        }

        private void CollectFirstUse(string clientId) =>
            Send(clientId, new FirstUse
            {
                UsageId = nameof(FirstUse),
                OS = SystemInfo.operatingSystem,
                DeviceModel = SystemInfo.deviceModel,
                DeviceType = SystemInfo.deviceType.ToString(),
                DeviceId = SystemInfo.deviceUniqueIdentifier,
                ProcessorType = SystemInfo.processorType
            });

        private void CollectResourceUsage() =>
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(appUsageConfig.ResourceUsageCollectPeriodSeconds))
                .Hook(_ => Send(new ResourceUsage
                {
                    UsageId = nameof(ResourceUsage),
                    TotalReservedMemoryMb = AppUtils.ToMb(Profiler.GetTotalReservedMemoryLong()),
                    TotalAllocatedMemoryMb = AppUtils.ToMb(Profiler.GetTotalAllocatedMemoryLong()),
                    MonoHeapSizeMb = AppUtils.ToMb(Profiler.GetMonoHeapSizeLong()),
                    MonoUsedSizeMb = AppUtils.ToMb(Profiler.GetMonoUsedSizeLong())
                }))
                .AddTo(disposables);

        private void CollectErrorStatus(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception)
            {
                return;
            }
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Length > appUsageConfig.MaxStackTraceLength)
            {
                stackTrace = $"{stackTrace.Substring(0, appUsageConfig.MaxStackTraceLength)}...";
            }
            Send(new ErrorStatus
            {
                UsageId = nameof(ErrorStatus),
                ErrorMessage = logString,
                StackTrace = stackTrace,
                ErrorType = type.ToString()
            });
        }
    }
}
