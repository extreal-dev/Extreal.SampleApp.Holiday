using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.Integration.Messaging;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.P2P;
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
            if (appState.IsHost)
            {
                var groupConfig = new GroupConfig(groupName, assetHelper.MessagingConfig.MaxCapacity);
                await messagingClient.CreateGroupAsync(groupConfig);
            }

            var joiningConfig = new MessagingJoiningConfig(groupName);
            await messagingClient.JoinAsync(joiningConfig);
        }

        public async UniTaskVoid LeaveAsync()
        {
            if (appState.IsHost)
            {
                await messagingClient.DeleteGroupAsync(groupName);
            }
            else
            {
                await messagingClient.LeaveAsync();
            }
        }

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
