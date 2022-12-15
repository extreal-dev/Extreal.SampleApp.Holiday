using UnityEngine;

namespace Extreal.SampleApp.Holiday
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(MultiplayAppConfig),
        fileName = nameof(MultiplayAppConfig))]
    public class MultiplayAppConfig : ScriptableObject
    {
#pragma warning disable CC0052
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private ushort port = 7777;
#pragma warning restore CC0052

        public string Address => address;
        public ushort Port => port;
    }
}
