using UnityEngine;

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

        [SerializeField] private string multiplayConnectionApprovalRejectedErrorMessage;
        [SerializeField] private string multiplayUnexpectedDisconnectedErrorMessage;
        [SerializeField] private string multiplayConnectFailedErrorMessage;
        [SerializeField] private string chatUnexpectedDisconnectedErrorMessage;
        [SerializeField] private string chatConnectFailedErrorMessage;

        public string AvatarSelectionTitle => avatarSelectionTitle;

        public string TextChatSendButtonLabel => textChatSendButtonLabel;
        public string VoiceChatMuteOnButtonLabel => voiceChatMuteOnButtonLabel;
        public string VoiceChatMuteOffButtonLabel => voiceChatMuteOffButtonLabel;
        public string AvatarSelectionGoButtonLabel => avatarSelectionGoButtonLabel;
        public string VirtualSpaceBackButtonLabel => virtualSpaceBackButtonLabel;

        public string MultiplayConnectionApprovalRejectedErrorMessage =>
            multiplayConnectionApprovalRejectedErrorMessage;

        public string MultiplayUnexpectedDisconnectedErrorMessage => multiplayUnexpectedDisconnectedErrorMessage;
        public string MultiplayConnectFailedErrorMessage => multiplayConnectFailedErrorMessage;
        public string ChatUnexpectedDisconnectedErrorMessage => chatUnexpectedDisconnectedErrorMessage;
        public string ChatConnectFailedErrorMessage => chatConnectFailedErrorMessage;
    }
}
