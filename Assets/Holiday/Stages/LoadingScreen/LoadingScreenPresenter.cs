namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using System;
    using App;
    using VContainer;
    using VContainer.Unity;

    public class LoadingScreenPresenter : IInitializable, IDisposable
    {
        [Inject] private StageNavigator stageNavigator;

        [Inject] private LoadingScreenView loadingScreenView;

        public void Initialize()
        {
            stageNavigator.OnLoading += OnStageLoading;
            stageNavigator.OnLoaded += OnStageLoaded;
        }

        public void Dispose()
        {
            stageNavigator.OnLoading -= OnStageLoading;
            stageNavigator.OnLoaded -= OnStageLoaded;
            GC.SuppressFinalize(this);
        }

        private void OnStageLoading(StageName stageName) => loadingScreenView.Show(stageName);

        private void OnStageLoaded(StageName stageName) => loadingScreenView.Hide(stageName);
    }
}
