using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.NotificationControl
{
    public class NotificationControlPresenter : StagePresenterBase
    {
        private readonly NotificationControlView notificationControlView;
        private readonly AppState appState;

        public NotificationControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            NotificationControlView notificationControlView,
            AppState appState) : base(stageNavigator)
        {
            this.notificationControlView = notificationControlView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            appState.OnNotificationReceived
                .Subscribe(notificationControlView.Show)
                .AddTo(sceneDisposables);

            notificationControlView.OnOkButtonClicked
                .Subscribe(_ => notificationControlView.Hide())
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
