using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.Retry;
using Extreal.Integration.Chat.Vivox;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(ChatConfig),
        fileName = nameof(ChatConfig))]
    public class ChatConfig : ScriptableObject
    {
        [SerializeField] private string apiEndPoint;
        [SerializeField] private string domain;
        [SerializeField] private string issuer;
        [SerializeField] private string secretKey;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int maxRetryCount = 8;

        public VivoxAppConfig VivoxAppConfig
        {
            get
            {
                var retryStrategy = new CountingRetryStrategy(maxRetryCount);
                return new VivoxAppConfig(apiEndPoint, domain, issuer, secretKey, loginRetryStrategy: retryStrategy);
            }
        }
    }
}
