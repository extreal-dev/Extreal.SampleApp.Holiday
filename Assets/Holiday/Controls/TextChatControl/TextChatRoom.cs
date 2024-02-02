using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.Messaging;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.P2P;
using SocketIOClient;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatRoom : DisposableBase
    {
        public IObservable<string> OnMessageReceived => onMessageReceived;
        private readonly Subject<string> onMessageReceived = new Subject<string>();

        private readonly MessagingClient messagingClient;
        private readonly AppState appState;
        private readonly AssetHelper assetHelper;

        private string groupName;

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(TextChatRoom));

        public TextChatRoom(MessagingClient messagingClient, AppState appState, AssetHelper assetHelper)
        {
            this.messagingClient = messagingClient;
            this.appState = appState;
            this.assetHelper = assetHelper;

            onMessageReceived.AddTo(disposables);

            this.messagingClient.OnMessageReceived
                .Subscribe(values =>
                {
                    var message = JsonUtility.FromJson<Message>(values.message);
                    if (message.MessageId == MessageId.TextChat)
                    {
                        var messageContent = (TextChatMessageContent)message.Content;
                        onMessageReceived.OnNext(messageContent.MessageContent);
                        return;
                    }

                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug(
                            "Received everyone message" + Environment.NewLine
                            + $" message ID: {message.MessageId}" + Environment.NewLine
                            + $" parameter: {message.Content}");
                    }

                    appState.ReceivedMessage(message);
                })
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
            => disposables.Dispose();

        public async UniTaskVoid JoinAsync()
        {
            groupName = $"TextChat#{appState.GroupName}";
            var joiningConfig = new MessagingJoiningConfig(groupName);
            ;
            try
            {
                if (appState.IsHost)
                {
                    var groups = await messagingClient.ListGroupsAsync();
                    var groupNames = groups.Select(group => group.Name).ToList();
                    if (groupNames.Contains(joiningConfig.GroupName))
                    {
                        appState.Notify(assetHelper.MessageConfig.TextChatMessagingGroupNameAlreadyExistsMessage);
                    }
                    else
                    {
                        await messagingClient.JoinAsync(joiningConfig);
                    }
                }
                else
                {
                    await messagingClient.JoinAsync(joiningConfig);
                }
            }
            catch (ConnectionException)
            {
                appState.Notify(assetHelper.MessageConfig.TextChatUnexpectedDisconnectedMessage);
            }

        }

        public async UniTaskVoid LeaveAsync() => await messagingClient.LeaveAsync();

        public async UniTaskVoid SendMessageAsync(string message)
        {
            var messageJson = JsonUtility.ToJson(new Message(MessageId.TextChat, new TextChatMessageContent(message)));
            await messagingClient.SendMessageAsync(messageJson);
        }

        public void SendEveryoneMessageAsync(Message message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(
                    "Send everyone message" + Environment.NewLine
                    + $" message ID: {message.MessageId}" + Environment.NewLine
                    + $" content: {message.Content}");
            }
            var messageJson = JsonUtility.ToJson(message);
            messagingClient.SendMessageAsync(messageJson).Forget();
        }
    }
}