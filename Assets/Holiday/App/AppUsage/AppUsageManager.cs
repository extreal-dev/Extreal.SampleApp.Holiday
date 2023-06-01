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
            disposables.Dispose();

            if (!appUsageConfig.Enable)
            {
                return;
            }
            Application.logMessageReceived -= CollectErrorStatus;
            Application.wantsToQuit -= WantsToQuit;
        }

        public void CollectAppUsage()
        {
            if (!appUsageConfig.Enable)
            {
                return;
            }
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
                .Hook(_ => CollectStageUsage())
                .AddTo(disposables);

            Application.wantsToQuit += WantsToQuit;
        }

        private void CollectStageUsage()
        {
            if (appState.StageState == null)
            {
                return;
            }
            Collect(appState.StageState.ToStageUsage());
        }

        private bool WantsToQuit()
        {
            CollectStageUsage();
            return true;
        }

        private void Collect(AppUsageBase appUsageBase)
            => Logger.LogInfo(AppUsageUtils.ToJson(appUsageBase, GetClientId(), appState));

        private void Collect(string clientId, AppUsageBase appUsageBase)
            => Logger.LogInfo(AppUsageUtils.ToJson(appUsageBase, clientId, appState));

        private string GetClientId()
        {
            var clientId = AppUsageUtils.GetClientId(appUsageConfig);
            if (clientId.IsGenerated)
            {
                CollectFirstUse(clientId.Value);
            }
            return clientId.Value;
        }

        private void CollectFirstUse(string clientId) =>
            Collect(clientId, new FirstUse
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
                .Hook(_ => Collect(new ResourceUsage
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
            Collect(ErrorStatus.Of(logString, stackTrace, type, appUsageConfig));
        }
    }
}
