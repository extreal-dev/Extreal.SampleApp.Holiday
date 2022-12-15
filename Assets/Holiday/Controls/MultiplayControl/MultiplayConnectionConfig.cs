using UnityEngine;

namespace Extreal.SampleApp.Holiday
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(MultiplayConnectionConfig),
        fileName = nameof(MultiplayConnectionConfig))]
    public class MultiplayConnectionConfig : ScriptableObject
    {
        [SerializeField] private string address;
        [SerializeField] private ushort port;

        public string Address => address;
        public ushort Port => port;
    }
}
