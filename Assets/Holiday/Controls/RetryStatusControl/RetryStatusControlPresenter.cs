using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.RetryStatusControl
{
    public class RetryStatusControlPresenter : StagePresenterBase
    {
        private readonly RetryStatusControlView retryStatusControlView;

        public RetryStatusControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            RetryStatusControlView retryStatusControlView) : base(stageNavigator, appState)
            => this.retryStatusControlView = retryStatusControlView;

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
            => appState.OnRetryStatusReceived
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

        private async UniTaskVoid HandleSuccessAsync(RetryStatus status)
        {
            retryStatusControlView.Show(status.Message);
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            retryStatusControlView.Hide();
        }
    }
}
