namespace Extreal.SampleApp.Holiday.Stages.BackgroundScreen
{
    using App;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class BackgroundScreenPresenter : IInitializable
    {
        [Inject]
        private StageNavigator stageNavigator;

        [Inject]
        private BackgroundScreenView backgroundScreenView;

        public void Initialize()
        {
            backgroundScreenView.Show();
            stageNavigator.OnLoading += OnStageLoading;
            stageNavigator.OnLoaded += OnStageLoaded;
        }

        private void OnStageLoading(StageName stageName) => backgroundScreenView.Show();

        private void OnStageLoaded(StageName stageName)
        {
            if (stageName == StageName.VirtualSpace)
            {
                backgroundScreenView.Hide();
            }
        }
    }
}
