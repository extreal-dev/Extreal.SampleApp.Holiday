using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly LoadingScreenView loadingScreenView;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public LoadingScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            LoadingScreenView loadingScreenView,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.loadingScreenView = loadingScreenView;
            this.appState = appState;
        }

        public void Initialize()
        {
            stageNavigator.OnStageTransitioning
                .Subscribe(OnStageTransitioning)
                .AddTo(disposables);

            appState.IsPlaying
                .Subscribe(OnPlayingChangedAsync)
                .AddTo(disposables);

            appState.OnNotificationReceived
                .Subscribe(_ => loadingScreenView.Hide())
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageTransitioning(StageName stageName)
        {
            if (AppUtils.IsSpace(stageName))
            {
                loadingScreenView.Show();
            }
        }

        private async void OnPlayingChangedAsync(bool isPlaying)
        {
            if (isPlaying)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(200));
                loadingScreenView.Hide();
            }
        }
    }
}
