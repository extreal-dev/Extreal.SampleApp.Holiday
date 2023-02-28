using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Extreal.SampleApp.Holiday.Controls.VirtualSpaceControl
{
    public class VirtualSpaceControlPresenter : StagePresenterBase
    {
        private readonly VirtualSpaceControlView virtualSpaceControlView;
        private readonly AppState appState;
        private readonly AssetProvider assetProvider;

        private SceneInstance scene;

        public VirtualSpaceControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VirtualSpaceControlView virtualSpaceControlView,
            AppState appState,
            AssetProvider assetProvider
        ) : base(stageNavigator)
        {
            this.virtualSpaceControlView = virtualSpaceControlView;
            this.appState = appState;
            this.assetProvider = assetProvider;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            virtualSpaceControlView.OnBackButtonClicked
                .Subscribe(_ =>
                {
                    UnloadVirtualSceneAsync().Forget();
                    stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget();
                })
                .AddTo(sceneDisposables);

            LoadVirtualSceneAsync().Forget();
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }

        private async UniTask LoadVirtualSceneAsync()
        {
            scene = await assetProvider.LoadSceneAsync("VirtualSpace");
            appState.GotReadyToUseSpace();
        }

        private async UniTask UnloadVirtualSceneAsync()
            => await Addressables.UnloadSceneAsync(scene);
    }
}
