﻿using UnityEngine;

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

        [SerializeField] private string multiplayConnectionApprovalRejectedMessage;
        [SerializeField] private string multiplayConnectRetryMessage;
        [SerializeField] private string multiplayConnectRetrySuccessMessage;
        [SerializeField] private string multiplayConnectRetryFailureMessage;
        [SerializeField] private string multiplayUnexpectedDisconnectedMessage;

        [SerializeField] private string chatConnectRetryMessage;
        [SerializeField] private string chatConnectRetrySuccessMessage;
        [SerializeField] private string chatConnectRetryFailureMessage;
        [SerializeField] private string chatUnexpectedDisconnectedMessage;

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

        public string MultiplayConnectionApprovalRejectedMessage => multiplayConnectionApprovalRejectedMessage;
        public string MultiplayConnectRetryMessage => multiplayConnectRetryMessage;
        public string MultiplayConnectRetrySuccessMessage => multiplayConnectRetrySuccessMessage;
        public string MultiplayConnectRetryFailureMessage => multiplayConnectRetryFailureMessage;
        public string MultiplayUnexpectedDisconnectedMessage => multiplayUnexpectedDisconnectedMessage;

        public string ChatConnectRetryMessage => chatConnectRetryMessage;
        public string ChatConnectRetrySuccessMessage => chatConnectRetrySuccessMessage;
        public string ChatConnectRetryFailureMessage => chatConnectRetryFailureMessage;
        public string ChatUnexpectedDisconnectedMessage => chatUnexpectedDisconnectedMessage;

        public string LandscapeErrorMessage => landscapeErrorMessage;
    }
}
