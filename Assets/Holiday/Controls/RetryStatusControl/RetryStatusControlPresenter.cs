using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.RetryStatusControl
{
    public class RetryStatusControlPresenter : StagePresenterBase
    {
        private readonly RetryStatusControlView retryStatusControlView;
        private readonly AppState appState;

        private StageName stageName;

        public RetryStatusControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            RetryStatusControlView retryStatusControlView,
            AppState appState) : base(stageNavigator)
        {
            this.retryStatusControlView = retryStatusControlView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables) =>
            appState.OnRetryStatusReceived
                .Where(_ => AppUtils.IsSpace(stageName))
                .Subscribe(status =>
                {
                    if (status.State == RetryStatus.RunState.Retrying)
                    {
                        retryStatusControlView.Show(status.Message);
                    }
                    else if (status.State == RetryStatus.RunState.Success)
                    {
                        HandleSuccessAsync(status).Forget();
                    }
                    else
                    {
                        retryStatusControlView.Hide();
                    }
                })
                .AddTo(sceneDisposables);

        private async UniTask HandleSuccessAsync(RetryStatus status)
        {
            retryStatusControlView.Show(status.Message);
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            retryStatusControlView.Hide();
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName) => this.stageName = stageName;
    }
}
