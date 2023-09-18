using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using UniRx;

namespace Extreal.SampleApp.Holiday.Spaces.Common
{
    public abstract class SpacePresenterBase : StagePresenterBase
    {
        protected SpacePresenterBase
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
        }

        protected override void Initialize
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables
        )
        {
        }

        protected sealed override async void OnStageEntered
        (
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables
        )
        {
            await OnStageEnteredWithoutForgetAsync(stageName, appState, stageDisposables);
            appState.SetSpaceReady(true);
        }

        protected sealed override async void OnStageExiting(StageName stageName, AppState appState)
        {
            await OnStageExitingWithoutForgetAsync(stageName, appState);
            appState.SetSpaceReady(false);
        }

#pragma warning disable CS1998
        protected virtual async UniTask OnStageEnteredWithoutForgetAsync
        (
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables
        )
        {
        }
#pragma warning restore CS1998

#pragma warning disable CS1998
        protected virtual async UniTask OnStageExitingWithoutForgetAsync(StageName stageName, AppState appState)
        {
        }
#pragma warning restore CS1998
    }
}
