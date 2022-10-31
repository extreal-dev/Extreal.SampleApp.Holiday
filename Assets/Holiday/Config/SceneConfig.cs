using UnityEngine;

namespace Extreal.SampleApp.Holiday.Common
{
    using System.Collections.Generic;
    using Core.SceneTransition;

    [CreateAssetMenu(
        menuName = "Config/" + nameof(SceneConfig),
        fileName = nameof(SceneConfig))]
    public class SceneConfig: ScriptableObject, ISceneConfig<SceneName, UnitySceneName>
    {
        [SerializeField] private List<UnitySceneName> commonUnitySceneNames;
        [SerializeField] private List<Scene<SceneName, UnitySceneName>> scenes;

        public List<UnitySceneName> CommonUnitySceneNames => commonUnitySceneNames;
        public List<Scene<SceneName, UnitySceneName>> Scenes => scenes;
    }
}
