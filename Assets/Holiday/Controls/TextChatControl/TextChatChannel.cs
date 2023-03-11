using System;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.Controls.Common;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatChannel : ChatChannelBase
    {
        public IObservable<string> OnMessageReceived
            => vivoxClient.OnTextMessageReceived.Select(channelTextMessage => channelTextMessage.Message);

        private readonly VivoxClient vivoxClient;

        public TextChatChannel(VivoxClient vivoxClient, string channelName) : base(vivoxClient, channelName)
            => this.vivoxClient = vivoxClient;

        protected override UniTask<ChannelId> ConnectAsync(string channelName)
            => vivoxClient.ConnectAsync(new VivoxChannelConfig(channelName, ChatType.TextOnly, transmissionSwitch: false));

        public void SendMessage(string message)
            => vivoxClient.SendTextMessage(message, ChannelId);
    }
}
