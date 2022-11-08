namespace Extreal.SampleApp.Holiday.App
{
    using System.Collections.Generic;
    using Core.SceneTransition;
    using UnityEngine;

    [CreateAssetMenu(
        menuName = "Config/" + nameof(StageConfig),
        fileName = nameof(StageConfig))]
    public class StageConfig : ScriptableObject, ISceneConfig<StageName, SceneName>
    {
        [SerializeField] private List<SceneName> commonScenes;
        [SerializeField] private List<Scene<StageName, SceneName>> stages;

        public List<SceneName> CommonUnitySceneNames => commonScenes;
        public List<Scene<StageName, SceneName>> Scenes => stages;
    }
}
