using System;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatChannel : ChatChannelBase
    {
        public IObservable<string> OnMessageReceived
            => vivoxClient.OnTextMessageReceived.Select(channelTextMessage => channelTextMessage.Message);

        private readonly VivoxClient vivoxClient;

        public TextChatChannel(VivoxClient vivoxClient, string channelName) : base(vivoxClient, channelName)
            => this.vivoxClient = vivoxClient;

        protected override void Connect(string channelName)
            => vivoxClient.Connect(new VivoxChannelConfig(channelName, ChatType.TextOnly, transmissionSwitch: false));

        public void SendMessage(string message)
            => vivoxClient.SendTextMessage(message, ChannelId);
    }
}
