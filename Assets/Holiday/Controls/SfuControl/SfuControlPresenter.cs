using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.Integration.SFU.OME;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.SfuControl
{
    public class SfuControlPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(SfuControlPresenter));

        private readonly AssetHelper assetHelper;
        private readonly OmeClient omeClient;

        public SfuControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            OmeClient omeClient) : base(stageNavigator, appState)
        {
            this.assetHelper = assetHelper;
            this.omeClient = omeClient;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables)
        {
            omeClient.OnJoined
                .Subscribe(_ => appState.SetSfuReady(true))
                .AddTo(sceneDisposables);

            omeClient.OnLeft
                .Subscribe(reason =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"Left room of SFU: reason={reason}");
                    }
                    appState.SetSfuReady(false);
                })
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(
            StageName stageName, AppState appState, CompositeDisposable stageDisposables)
        {
            if (appState.SfuReady.Value)
            {
                return;
            }
            omeClient.JoinAsync(appState.GroupName).Forget();
        }

        protected override void OnStageExiting(StageName stageName, AppState appState)
        {
            if (AppUtils.IsSpace(stageName))
            {
                return;
            }
            omeClient.LeaveAsync().Forget();
            appState.SetSfuReady(false);
        }
    }
}
