using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(MessageConfig),
        fileName = nameof(MessageConfig))]
    public class MessageConfig : ScriptableObject
    {
        [SerializeField] private string avatarSelectionTitle;
        [SerializeField] private string groupSelectionTitle;

        [SerializeField] private string textChatSendButtonLabel;
        [SerializeField] private string voiceChatMuteOnButtonLabel;
        [SerializeField] private string voiceChatMuteOffButtonLabel;
        [SerializeField] private string avatarSelectionGoButtonLabel;
        [SerializeField] private string groupSelectionUpdateButtonLabel;
        [SerializeField] private string groupSelectionGoButtonLabel;
        [SerializeField] private string groupSelectionBackButtonLabel;

        [SerializeField] private string spaceGoButtonLabel;
        [SerializeField] private string spaceBackButtonLabel;

        [SerializeField] private string groupMatchingUpdateFailureMessage;

        [SerializeField] private string p2PStartFailureMessage;
        [SerializeField] private string p2PUnexpectedDisconnectedMessage;
        [SerializeField] private string p2PHostNameAlreadyExistsMessage;

        [SerializeField] private string multiplayConnectionApprovalRejectedMessage;
        [SerializeField] private string multiplayUnexpectedDisconnectedMessage;

        [SerializeField] private string landscapeErrorMessage;

        public string AvatarSelectionTitle => avatarSelectionTitle;
        public string GroupSelectionTitle => groupSelectionTitle;

        public string TextChatSendButtonLabel => textChatSendButtonLabel;
        public string VoiceChatMuteOnButtonLabel => voiceChatMuteOnButtonLabel;
        public string VoiceChatMuteOffButtonLabel => voiceChatMuteOffButtonLabel;
        public string AvatarSelectionGoButtonLabel => avatarSelectionGoButtonLabel;
        public string GroupSelectionUpdateButtonLabel => groupSelectionUpdateButtonLabel;
        public string GroupSelectionGoButtonLabel => groupSelectionGoButtonLabel;
        public string GroupSelectionBackButtonLabel => groupSelectionBackButtonLabel;

        public string SpaceGoButtonLabel => spaceGoButtonLabel;
        public string SpaceBackButtonLabel => spaceBackButtonLabel;

        public string GroupMatchingUpdateFailureMessage => groupMatchingUpdateFailureMessage;

        public string P2PStartFailureMessage => p2PStartFailureMessage;
        public string P2PUnexpectedDisconnectedMessage => p2PUnexpectedDisconnectedMessage;
        public string P2PHostNameAlreadyExistsMessage => p2PHostNameAlreadyExistsMessage;

        public string MultiplayConnectionApprovalRejectedMessage => multiplayConnectionApprovalRejectedMessage;
        public string MultiplayUnexpectedDisconnectedMessage => multiplayUnexpectedDisconnectedMessage;

        public string LandscapeErrorMessage => landscapeErrorMessage;
    }
}
