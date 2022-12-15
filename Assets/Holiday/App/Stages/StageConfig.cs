using Extreal.Core.StageNavigation;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(StageConfig),
        fileName = nameof(StageConfig))]
    public class StageConfig : StageConfigBase<StageName, SceneName>
    {
    }
}
