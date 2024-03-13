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

        [SerializeField] private string omeUnexpectedDisconnectedMessage;
        [SerializeField] private string omeJoinRetryMessage;
        [SerializeField] private string omeJoinRetrySuccessMessage;
        [SerializeField] private string omeJoinRetryFailureMessage;

        [SerializeField] private string textChatMessagingGroupNameAlreadyExistsMessage;
        [SerializeField] private string textChatUnexpectedDisconnectedMessage;

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

        public string OmeUnexpectedDisconnectedMessage => omeUnexpectedDisconnectedMessage;
        public string OmeJoinRetryMessage => omeJoinRetryMessage;
        public string OmeJoinRetrySuccessMessage => omeJoinRetrySuccessMessage;
        public string OmeJoinRetryFailureMessage => omeJoinRetryFailureMessage;

        public string TextChatMessagingGroupNameAlreadyExistsMessage => textChatMessagingGroupNameAlreadyExistsMessage;
        public string TextChatUnexpectedDisconnectedMessage => textChatUnexpectedDisconnectedMessage;

        public string MultiplayConnectionApprovalRejectedMessage => multiplayConnectionApprovalRejectedMessage;
        public string MultiplayUnexpectedDisconnectedMessage => multiplayUnexpectedDisconnectedMessage;

        public string LandscapeErrorMessage => landscapeErrorMessage;
    }
}
