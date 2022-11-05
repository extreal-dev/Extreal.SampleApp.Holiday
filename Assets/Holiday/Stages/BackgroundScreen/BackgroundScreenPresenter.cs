namespace Extreal.SampleApp.Holiday.Stages.BackgroundScreen
{
    using System;
    using App;
    using VContainer;
    using VContainer.Unity;

    public class BackgroundScreenPresenter : IInitializable, IDisposable
    {
        [Inject]
        private StageNavigator stageNavigator;

        [Inject]
        private BackgroundScreenView backgroundScreenView;

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

        private void OnStageLoading(StageName stageName) => backgroundScreenView.Show();

        private void OnStageLoaded(StageName stageName) => backgroundScreenView.Hide();
    }
}
