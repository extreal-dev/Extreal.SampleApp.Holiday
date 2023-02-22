using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Extreal.SampleApp.Holiday.Controls.VirtualSpaceControl
{
    public class VirtualSpaceControlPresenter : StagePresenterBase
    {
        private readonly VirtualSpaceControlView virtualSpaceControlView;
        private readonly AppState appState;

        private SceneInstance scene;

        public VirtualSpaceControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VirtualSpaceControlView virtualSpaceControlView,
            AppState appState
        ) : base(stageNavigator)
        {
            this.virtualSpaceControlView = virtualSpaceControlView;
            this.appState = appState;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            virtualSpaceControlView.OnBackButtonClicked
                .Subscribe(async _ =>
                {
                    await UnloadVirtualSceneAsync();
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

        private async UniTaskVoid LoadVirtualSceneAsync()
        {
            scene = await Addressables.LoadSceneAsync("VirtualSpace", LoadSceneMode.Additive).Task;
            appState.GotReadyToUseSpace();
        }

        private async UniTask UnloadVirtualSceneAsync()
            => _ = await Addressables.UnloadSceneAsync(scene).Task;
    }
}
