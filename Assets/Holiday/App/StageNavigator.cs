namespace Extreal.SampleApp.Holiday.App
{
    using System;
    using Core.SceneTransition;
    using Cysharp.Threading.Tasks;
    using VContainer;

    /// <summary>
    /// ステージ切り替えの間、Loadingを出すために設けたクラス。
    /// TODO: Extrealのモジュール側にこのクラスの処理を移動する。
    /// </summary>
    public class StageNavigator
    {
        [Inject] private ISceneTransitioner<StageName> sceneTransitioner;

        public event Action<StageName> OnLoading;
        public event Action<StageName> OnLoaded;

        public async UniTask ReplaceAsync(StageName stageName)
        {
            OnLoading?.Invoke(stageName);
            await sceneTransitioner.ReplaceAsync(stageName);
            OnLoaded?.Invoke(stageName);
        }
    }
}
