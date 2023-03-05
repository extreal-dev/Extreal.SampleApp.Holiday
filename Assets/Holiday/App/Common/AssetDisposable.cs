using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Extreal.SampleApp.Holiday.App.Common
{
    /// <summary>
    /// Extreal.Integration.Assets.Addressablesモジュールに入るクラス。
    /// </summary>
    public class AssetDisposable<TResult> : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AssetDisposable<TResult>));

        public TResult Result { get; }

        internal AssetDisposable(TResult result) => Result = result;

        protected override void ReleaseManagedResources()
        {
            if (Result is SceneInstance result)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Unload scene: {result.Scene.name}");
                }
                Addressables.UnloadSceneAsync(result);
            }
            else
            {
                if (Logger.IsDebug())
                {
                    var name = Result is GameObject gameObject ? gameObject.name : Result.ToString();
                    Logger.LogDebug($"Release: {name}");
                }
                Addressables.Release(Result);
            }
        }
    }
}
