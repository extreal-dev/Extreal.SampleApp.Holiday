using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.MultiplayClient.App;
using Extreal.SampleApp.Holiday.MultiplayClient.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Screens.LoadingScreen
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
