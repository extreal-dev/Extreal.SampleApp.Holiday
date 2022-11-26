using System;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.Models;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Stages.BackgroundScreen
{
    public class BackgroundScreenPresenter : IInitializable, IDisposable
    {
        [Inject] private IStageNavigator<StageName> stageNavigator;
        [Inject] private BackgroundScreenView backgroundScreenView;
        [Inject] private Player player;

        private CompositeDisposable compositeDisposable = new();

        public void Initialize()
        {
            stageNavigator.OnStageTransitioning += OnStageTransitioning;
            stageNavigator.OnStageTransitioned += OnStageTransitioned;
            player.IsPlaying.Subscribe(OnPlayerPlayingChanged).AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            stageNavigator.OnStageTransitioning -= OnStageTransitioning;
            stageNavigator.OnStageTransitioned -= OnStageTransitioned;
            compositeDisposable?.Dispose();
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
