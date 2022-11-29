using System;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Stages.BackgroundScreen
{
    public class BackgroundScreenPresenter : IInitializable, IDisposable
    {
        private readonly IStageNavigator<StageName> stageNavigator;
        private readonly BackgroundScreenView backgroundScreenView;
        private readonly Player player;

        public BackgroundScreenPresenter(IStageNavigator<StageName> stageNavigator,
            BackgroundScreenView backgroundScreenView, Player player)
        {
            this.stageNavigator = stageNavigator;
            this.backgroundScreenView = backgroundScreenView;
            this.player = player;
        }

        private readonly CompositeDisposable disposable = new CompositeDisposable();

        public void Initialize()
        {
            stageNavigator.OnStageTransitioning += OnStageTransitioning;
            stageNavigator.OnStageTransitioned += OnStageTransitioned;
            player.IsPlaying.Subscribe(OnPlayerPlayingChanged).AddTo(disposable);
        }

        public void Dispose()
        {
            stageNavigator.OnStageTransitioning -= OnStageTransitioning;
            stageNavigator.OnStageTransitioned -= OnStageTransitioned;
            disposable.Dispose();
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

        private void OnPlayerPlayingChanged(bool isPlaying)
        {
            if (isPlaying)
            {
                backgroundScreenView.Hide();
            }
        }
    }
}
