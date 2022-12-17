using Extreal.Integration.Multiplay.NGO;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(MultiplayConfig),
        fileName = nameof(MultiplayConfig))]
    public class MultiplayConfig : ScriptableObject
    {
#pragma warning disable CC0052
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private ushort port = 7777;
#pragma warning restore CC0052

        public NgoConfig ToNgoConfig() => new NgoConfig(address, port);
    }
}
