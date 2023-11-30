using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.InputControl
{
    public class InputControlPresenter : StagePresenterBase
    {
        private readonly InputControlView inputControlView;

        public InputControlPresenter
        (
            InputControlView inputControlView,
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
            => this.inputControlView = inputControlView;

        protected override void OnStageEntered(StageName stageName, AppState appState, CompositeDisposable stageDisposables)
            => inputControlView.SwitchJoystickVisibility(AppUtils.IsSpace(stageName) && AppUtils.IsTouchDevice());
    }
}
