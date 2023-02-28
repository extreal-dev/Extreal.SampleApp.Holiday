using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.ConfirmationScreen
{
    public class ConfirmationScreenPresenter : StagePresenterBase
    {
        private readonly ConfirmationScreenView confirmationScreenView;
        private readonly AppState appState;

        public ConfirmationScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            ConfirmationScreenView confirmationScreenView,
            AppState appState
        ) : base(stageNavigator)
        {
            this.confirmationScreenView = confirmationScreenView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            var confirmation = default(Confirmation);

            appState.OnConfirmationReceived
                .Subscribe(c =>
                {
                    confirmation = c;
                    confirmationScreenView.Show(c.Message);
                })
                .AddTo(sceneDisposables);

            confirmationScreenView.OkButtonClicked
                .Subscribe(_ =>
                {
                    confirmationScreenView.Hide();
                    confirmation.OkAction?.Invoke();
                })
                .AddTo(sceneDisposables);

            confirmationScreenView.CancelButtonClicked
                .Subscribe(_ =>
                {
                    confirmationScreenView.Hide();
                    confirmation.CancelAction?.Invoke();
                })
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
