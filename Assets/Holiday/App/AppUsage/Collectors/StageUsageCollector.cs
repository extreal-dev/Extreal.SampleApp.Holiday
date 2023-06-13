using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Hook;
using UniRx;

namespace Extreal.SampleApp.Holiday.App.AppUsage.Collectors
{
    public class StageUsageCollector: IAppUsageCollector
    {
        public IDisposable Collect(AppUsageManager appUsageManager)
        {
            Action collect = () =>
            {
                var stageState = appUsageManager.AppState.StageState;
                if (stageState == null)
                {
                    return;
                }
                appUsageManager.Collect(new StageUsage
                {
                    UsageId = nameof(StageUsage),
                    StayTimeSeconds = stageState.StayTimeSeconds,
                    NumberOfTextChatsSent = stageState.NumberOfTextChatsSent
                });
            };

            var disposables = new CompositeDisposable();

            appUsageManager.StageNavigator.OnStageTransitioning
                .Hook(_ => collect())
                .AddTo(disposables);

            appUsageManager.OnApplicationExiting
                .Hook(_ => collect())
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
