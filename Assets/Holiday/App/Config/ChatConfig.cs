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

        public VivoxAppConfig ToVivoxAppConfig() => new VivoxAppConfig(apiEndPoint, domain, issuer, secretKey);
    }
}
