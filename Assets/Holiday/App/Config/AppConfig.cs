namespace Extreal.SampleApp.Holiday.App.Config
{
    public class AppConfig
    {
        public string AvatarSelectionTitle { get; }

        public string TextChatSendButtonLabel { get; }
        public string VoiceChatMuteOnButtonLabel { get; }
        public string VoiceChatMuteOffButtonLabel { get; }
        public string AvatarSelectionGoButtonLabel { get; }
        public string VirtualSpaceBackButtonLabel { get; }

        public string MultiplayConnectionApprovalRejectedErrorMessage { get; }
        public string MultiplayUnexpectedDisconnectedErrorMessage { get; }
        public string MultiplayConnectFailedErrorMessage { get; }
        public string ChatUnexpectedDisconnectedErrorMessage { get; }
        public string ChatConnectFailedErrorMessage { get; }

        public AppConfig
        (
            string avatarSelectionTitle,
            string textChatSendButtonLabel,
            string voiceChatMuteOnButtonLabel,
            string voiceChatMuteOffButtonLabel,
            string avatarSelectionGoButtonLabel,
            string virtualSpaceBackButtonLabel,
            string multiplayConnectionApprovalRejectedErrorMessage,
            string multiplayUnexpectedDisconnectedErrorMessage,
            string multiplayConnectFailedErrorMessage,
            string chatUnexpectedDisconnectedErrorMessage,
            string chatConnectFailedErrorMessage
        )
        {
            AvatarSelectionTitle = avatarSelectionTitle;
            TextChatSendButtonLabel = textChatSendButtonLabel;
            VoiceChatMuteOnButtonLabel = voiceChatMuteOnButtonLabel;
            VoiceChatMuteOffButtonLabel = voiceChatMuteOffButtonLabel;
            AvatarSelectionGoButtonLabel = avatarSelectionGoButtonLabel;
            VirtualSpaceBackButtonLabel = virtualSpaceBackButtonLabel;
            MultiplayConnectionApprovalRejectedErrorMessage = multiplayConnectionApprovalRejectedErrorMessage;
            MultiplayUnexpectedDisconnectedErrorMessage = multiplayUnexpectedDisconnectedErrorMessage;
            MultiplayConnectFailedErrorMessage = multiplayConnectFailedErrorMessage;
            ChatUnexpectedDisconnectedErrorMessage = chatUnexpectedDisconnectedErrorMessage;
            ChatConnectFailedErrorMessage = chatConnectFailedErrorMessage;
        }
    }
}
