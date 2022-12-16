using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(MultiplayServerConfig),
        fileName = nameof(MultiplayServerConfig))]
    public class MultiplayServerConfig : ScriptableObject
    {
        [SerializeField] private int maxCapacity;

        public int MaxCapacity => maxCapacity;
    }
}
