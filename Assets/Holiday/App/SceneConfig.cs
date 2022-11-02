namespace Extreal.SampleApp.Holiday.App
{
    using UnityEngine;
    using System.Collections.Generic;
    using Core.SceneTransition;

    [CreateAssetMenu(
        menuName = "Config/" + nameof(SceneConfig),
        fileName = nameof(SceneConfig))]
    public class SceneConfig: ScriptableObject, ISceneConfig<StageName, SceneName>
    {
        [SerializeField] private List<SceneName> commonUnitySceneNames;
        [SerializeField] private List<Scene<StageName, SceneName>> scenes;

        public List<SceneName> CommonUnitySceneNames => commonUnitySceneNames;
        public List<Scene<StageName, SceneName>> Scenes => scenes;
    }
}
