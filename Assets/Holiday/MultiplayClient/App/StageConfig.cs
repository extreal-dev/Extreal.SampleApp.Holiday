using System.Collections.Generic;
using Extreal.Core.StageNavigation;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayClient.App
{
    [CreateAssetMenu(
        menuName = "Config/" + nameof(StageConfig),
        fileName = nameof(StageConfig))]
    public class StageConfig : ScriptableObject, IStageConfig<StageName, SceneName>
    {
        [SerializeField] private List<SceneName> commonScenes;
        [SerializeField] private List<Stage<StageName, SceneName>> stages;

        public List<SceneName> CommonScenes => commonScenes;
        public List<Stage<StageName, SceneName>> Stages => stages;
    }
}
