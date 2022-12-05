using System;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.MultiplayClient.App;
using Extreal.SampleApp.Holiday.MultiplayClient.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Screens.BackgroundScreen
{
    public class BackgroundScreenPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly BackgroundScreenView backgroundScreenView;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public BackgroundScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            BackgroundScreenView backgroundScreenView,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.backgroundScreenView = backgroundScreenView;
            this.appState = appState;
        }


        public void Initialize()
        {
            stageNavigator.OnStageTransitioning
                .Subscribe(OnStageTransitioning)
                .AddTo(disposables);

            stageNavigator.OnStageTransitioned
                .Subscribe(OnStageTransitioned)
                .AddTo(disposables);

            appState.IsPlaying
                .Subscribe(OnPlayingChanged)
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageTransitioning(StageName stageName) => backgroundScreenView.Show();

        private void OnStageTransitioned(StageName stageName)
        {
            if (!AppUtils.IsSpace(stageName))
            {
                backgroundScreenView.Hide();
            }
        }

        private void OnPlayingChanged(bool isPlaying)
        {
            if (isPlaying)
            {
                backgroundScreenView.Hide();
            }
        }
    }
}
