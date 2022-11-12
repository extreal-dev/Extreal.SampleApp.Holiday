namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using System;
    using App;
    using Cysharp.Threading.Tasks;
    using Models;
    using UniRx;
    using VContainer;
    using VContainer.Unity;

    public class LoadingScreenPresenter : IInitializable, IDisposable
    {
        [Inject] private StageNavigator stageNavigator;
        [Inject] private LoadingScreenView loadingScreenView;
        [Inject] private Player player;

        private readonly CompositeDisposable compositeDisposable = new();

        public void Initialize()
        {
            stageNavigator.OnLoading += OnStageLoading;
            player.IsPlaying.Subscribe(OnPlayerPlayingChangedAsync).AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            stageNavigator.OnLoading -= OnStageLoading;
            compositeDisposable?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageLoading(StageName stageName)
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
