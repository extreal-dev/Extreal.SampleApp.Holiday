using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(MultiplayConfig),
        fileName = nameof(MultiplayConfig))]
    public class MultiplayConfig : MessagingConfig
    {
    }
}
