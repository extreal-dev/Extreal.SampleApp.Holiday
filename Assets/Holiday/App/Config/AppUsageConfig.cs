using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(AppUsageConfig),
        fileName = nameof(AppUsageConfig))]
    public class AppUsageConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string clientIdKey = $"{nameof(Holiday)}_client_id";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int resourceUsageCollectPeriodSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int maxStackTraceLength = 500;

        public string ClientIdKey => clientIdKey;
        public int ResourceUsageCollectPeriodSeconds => resourceUsageCollectPeriodSeconds;
        public int MaxStackTraceLength => maxStackTraceLength;
    }
}
