using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(AppConfigRepository),
        fileName = nameof(AppConfigRepository))]
    public class AppConfigRepository : ScriptableObject
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

        public AppConfig ToAppConfig()
            => new AppConfig(
                avatarSelectionTitle,
                textChatSendButtonLabel,
                voiceChatMuteOnButtonLabel,
                voiceChatMuteOffButtonLabel,
                avatarSelectionGoButtonLabel,
                virtualSpaceBackButtonLabel,
                multiplayConnectionApprovalRejectedErrorMessage,
                multiplayUnexpectedDisconnectedErrorMessage,
                multiplayConnectFailedErrorMessage,
                chatUnexpectedDisconnectedErrorMessage,
                chatConnectFailedErrorMessage);
    }
}
