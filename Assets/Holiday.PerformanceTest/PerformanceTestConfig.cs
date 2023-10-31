using NUnit.Framework.Internal;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.PerformanceTest
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "." + nameof(PerformanceTest) + "/" + nameof(PerformanceTestConfig),
        fileName = nameof(PerformanceTestConfig))]
    public class PerformanceTestConfig : ScriptableObject
    {
        [SerializeField] private Role role;

        public Role Role => role;
    }

}
