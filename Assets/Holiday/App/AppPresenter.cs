using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.AppUsage;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppPresenter : DisposableBase, IInitializable, IAsyncStartable
    {
        private readonly AppConfig appConfig;
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AssetHelper assetHelper;
        private readonly AppState appState;
        private readonly AppUsageManager appUsageManager;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public AppPresenter(
            AppConfig appConfig,
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            AppUsageManager appUsageManager)
        {
            this.appConfig = appConfig;
            this.stageNavigator = stageNavigator;
            this.assetHelper = assetHelper;
            this.appState = appState;
            this.appUsageManager = appUsageManager;
        }

        public void Initialize()
        {
            assetHelper.OnConnectRetrying
                .Subscribe(retryCount => AppUtils.NotifyRetrying(
                    appState,
                    appConfig.DownloadConnectRetryMessage,
                    retryCount))
                .AddTo(disposables);

            assetHelper.OnConnectRetried
                .Subscribe(result => AppUtils.NotifyRetried(
                    appState,
                    result,
                    appConfig.DownloadRetrySuccessMessage,
                    appConfig.DownloadRetryFailureMessage))
                .AddTo(disposables);

            appUsageManager.CollectAppUsage();
        }

        public async UniTask StartAsync(CancellationToken cancellation)
            => await stageNavigator.ReplaceAsync(StageName.TitleStage);

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
