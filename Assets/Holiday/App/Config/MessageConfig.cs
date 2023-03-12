using UnityEngine;
using UnityEngine.Serialization;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(MessageConfig),
        fileName = nameof(MessageConfig))]
    public class MessageConfig : ScriptableObject
    {
        [SerializeField] private string avatarSelectionTitle;

        [SerializeField] private string textChatSendButtonLabel;
        [SerializeField] private string voiceChatMuteOnButtonLabel;
        [SerializeField] private string voiceChatMuteOffButtonLabel;
        [SerializeField] private string avatarSelectionGoButtonLabel;
        [SerializeField] private string virtualSpaceBackButtonLabel;

        [SerializeField] private string multiplayConnectionApprovalRejectedMessage;
        [SerializeField] private string multiplayConnectRetryMessage;
        [SerializeField] private string multiplayConnectRetrySuccessMessage;
        [SerializeField] private string multiplayConnectRetryFailureMessage;
        [SerializeField] private string multiplayUnexpectedDisconnectedMessage;

        [SerializeField] private string chatConnectRetryMessage;
        [SerializeField] private string chatConnectRetrySuccessMessage;
        [SerializeField] private string chatConnectRetryFailureMessage;
        [SerializeField] private string chatUnexpectedDisconnectedMessage;

        public string AvatarSelectionTitle => avatarSelectionTitle;

        public string TextChatSendButtonLabel => textChatSendButtonLabel;
        public string VoiceChatMuteOnButtonLabel => voiceChatMuteOnButtonLabel;
        public string VoiceChatMuteOffButtonLabel => voiceChatMuteOffButtonLabel;
        public string AvatarSelectionGoButtonLabel => avatarSelectionGoButtonLabel;
        public string VirtualSpaceBackButtonLabel => virtualSpaceBackButtonLabel;

        public string MultiplayConnectionApprovalRejectedMessage => multiplayConnectionApprovalRejectedMessage;
        public string MultiplayConnectRetryMessage => multiplayConnectRetryMessage;
        public string MultiplayConnectRetrySuccessMessage => multiplayConnectRetrySuccessMessage;
        public string MultiplayConnectRetryFailureMessage => multiplayConnectRetryFailureMessage;
        public string MultiplayUnexpectedDisconnectedMessage => multiplayUnexpectedDisconnectedMessage;

        public string ChatConnectRetryMessage => chatConnectRetryMessage;
        public string ChatConnectRetrySuccessMessage => chatConnectRetrySuccessMessage;
        public string ChatConnectRetryFailureMessage => chatConnectRetryFailureMessage;
        public string ChatUnexpectedDisconnectedMessage => chatUnexpectedDisconnectedMessage;
    }
}
