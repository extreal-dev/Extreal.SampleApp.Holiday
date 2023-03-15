using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(AppConfig),
        fileName = nameof(AppConfig))]
    public class AppConfig : ScriptableObject
    {
        [SerializeField] private int verticalSyncs = 0;
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private int addressablesTimeoutSeconds = 5;
        [SerializeField] private int addressablesMaxRetryCount = 6;

        public int VerticalSyncs => verticalSyncs;
        public int TargetFrameRate => targetFrameRate;
        public int AddressablesTimeoutSeconds => addressablesTimeoutSeconds;
        public int AddressablesMaxRetryCount => addressablesMaxRetryCount;
    }
}
