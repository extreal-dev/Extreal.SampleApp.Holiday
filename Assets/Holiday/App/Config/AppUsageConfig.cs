using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(AppUsageConfig),
        fileName = nameof(AppUsageConfig))]
    public class AppUsageConfig : ScriptableObject
    {
        [SerializeField, SuppressMessage("Usage", "CC0052")] private bool enable;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string orgId = "tenant1";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string pushUrl = "http://localhost:3100/loki/api/v1/push";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int timeoutSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string clientIdKey = $"{nameof(Holiday)}_client_id";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int resourceUsageCollectPeriodSeconds = 5;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int maxStackTraceLength = 500;

        public bool Enable => enable;
        public string OrgId => orgId;
        public string PushUrl => pushUrl;
        public int TimeoutSeconds => timeoutSeconds;
        public string ClientIdKey => clientIdKey;
        public int ResourceUsageCollectPeriodSeconds => resourceUsageCollectPeriodSeconds;
        public int MaxStackTraceLength => maxStackTraceLength;
    }
}
