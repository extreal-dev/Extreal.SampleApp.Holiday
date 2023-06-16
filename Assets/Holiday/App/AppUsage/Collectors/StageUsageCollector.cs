using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.App.AppUsage.Collectors
{
    public class StageUsageCollector: IAppUsageCollector
    {
        private readonly AppState appState;
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AppUsageEmitter appUsageEmitter;

        public StageUsageCollector(
            AppState appState, StageNavigator<StageName, SceneName> stageNavigator, AppUsageEmitter appUsageEmitter)
        {
            this.appState = appState;
            this.stageNavigator = stageNavigator;
            this.appUsageEmitter = appUsageEmitter;
        }

        public IDisposable Collect(Action<AppUsageBase> collect)
        {
            Action collectStageUsage = () =>
            {
                var stageState = appState.StageState;
                if (stageState == null)
                {
                    return;
                }
                collect?.Invoke(
                    new StageUsage
                    {
                        UsageId = nameof(StageUsage),
                        StayTimeSeconds = stageState.StayTimeSeconds,
                        NumberOfTextChatsSent = stageState.NumberOfTextChatsSent
                    });
            };

            var disposables = new CompositeDisposable();

            stageNavigator.OnStageTransitioning
                .Hook(_ => collectStageUsage())
                .AddTo(disposables);

            appUsageEmitter.OnApplicationExiting
                .Hook(_ => collectStageUsage())
                .AddTo(disposables);

            return disposables;
        }
    }

    [SuppressMessage("Usage", "IDE1006")]
    public class StageUsage : AppUsageBase
    {
        public long StayTimeSeconds;
        public int NumberOfTextChatsSent;
    }
}
