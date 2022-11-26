using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    public class LoadingScreenPresenter : IInitializable, IDisposable
    {
        private readonly IStageNavigator<StageName> stageNavigator;
        private readonly LoadingScreenView loadingScreenView;
        private readonly Player player;

        private readonly CompositeDisposable compositeDisposable = new();

        public LoadingScreenPresenter(IStageNavigator<StageName> stageNavigator, LoadingScreenView loadingScreenView,
            Player player)
        {
            this.stageNavigator = stageNavigator;
            this.loadingScreenView = loadingScreenView;
            this.player = player;
        }

        public void Initialize()
        {
            stageNavigator.OnStageTransitioning += OnStageTransitioning;
            player.IsPlaying.Subscribe(OnPlayerPlayingChangedAsync).AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            stageNavigator.OnStageTransitioning -= OnStageTransitioning;
            compositeDisposable?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageTransitioning(StageName stageName)
        {
            if (AppUtils.IsSpace(stageName))
            {
                loadingScreenView.Show();
            }
        }

        private async void OnPlayerPlayingChangedAsync(bool isPlaying)
        {
            if (isPlaying)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(200));
                loadingScreenView.Hide();
            }
        }
    }
}
