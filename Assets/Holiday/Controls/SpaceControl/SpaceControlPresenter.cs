using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Extreal.SampleApp.Holiday.Controls.SpaceControl
{
    public class SpaceControlPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(SpaceControlPresenter));

        private readonly SpaceControlView spaceControlView;
        private readonly AppState appState;
        private readonly AssetProvider assetProvider;

        private SceneInstance scene;

        public SpaceControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            SpaceControlView spaceControlView,
            AppState appState,
            AssetProvider assetProvider
        ) : base(stageNavigator)
        {
            this.spaceControlView = spaceControlView;
            this.appState = appState;
            this.assetProvider = assetProvider;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            appState.SpaceName
                .Subscribe(spaceName =>
                {
                    if (spaceName != null)
                    {
                        LoadSpaceAsync(spaceName).Forget();
                    }
                })
                .AddTo(sceneDisposables);

            spaceControlView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        private async UniTask LoadSpaceAsync(string spaceName)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Load space: {spaceName}");
            }
            scene = await assetProvider.LoadSceneAsync(spaceName);
            appState.SetSpaceReady(true);
        }

        protected override void OnStageExiting(StageName stageName)
        {
            appState.SetSpaceReady(false);
            Addressables.UnloadSceneAsync(scene);
        }
    }
}
