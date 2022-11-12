namespace Extreal.SampleApp.Holiday.Stages.BackgroundScreen
{
    using System;
    using App;
    using Models;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class BackgroundScreenPresenter : IInitializable, IDisposable
    {
        [Inject] private StageNavigator stageNavigator;
        [Inject] private BackgroundScreenView backgroundScreenView;
        [Inject] private Player player;

        private CompositeDisposable compositeDisposable = new();

        public void Initialize()
        {
            stageNavigator.OnLoading += OnStageLoading;
            stageNavigator.OnLoaded += OnStageLoaded;
            player.IsPlaying.Subscribe(OnPlayerPlayingChanged).AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            stageNavigator.OnLoading -= OnStageLoading;
            stageNavigator.OnLoaded -= OnStageLoaded;
            compositeDisposable?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageLoading(StageName stageName) => backgroundScreenView.Show();

        private void OnStageLoaded(StageName stageName)
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
