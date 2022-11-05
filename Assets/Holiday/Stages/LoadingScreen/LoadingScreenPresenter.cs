namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using System;
    using System.Collections.Generic;
    using App;
    using Core.Logging;
    using VContainer;
    using VContainer.Unity;

    public class LoadingScreenPresenter : IInitializable, IDisposable
    {
        [Inject] private readonly StageNavigator stageNavigator;

        [Inject] private readonly LoadingScreenView loadingScreenView;

        public void Initialize()
        {
            loadingScreenView.Hide();
            stageNavigator.OnLoading += OnStageLoading;
            stageNavigator.OnLoaded += OnStageLoaded;
        }

        public void Dispose()
        {
            stageNavigator.OnLoading -= OnStageLoading;
            stageNavigator.OnLoaded -= OnStageLoaded;
            GC.SuppressFinalize(this);
        }

        private readonly HashSet<StageName> stageNamesForLoading = new() { StageName.VirtualSpace };

        private void OnStageLoading(StageName stageName)
        {
            if (stageNamesForLoading.Contains(stageName))
            {
                loadingScreenView.Show();
            }
        }

        private void OnStageLoaded(StageName stageName)
        {
            if (stageNamesForLoading.Contains(stageName))
            {
                loadingScreenView.Hide();
            }
        }
    }
}
