using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(MultiplayConfig),
        fileName = nameof(MultiplayConfig))]
    public class MultiplayConfig : ScriptableObject
    {
        public int MaxCapacity => maxCapacity;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int maxCapacity = 100;
    }
}
