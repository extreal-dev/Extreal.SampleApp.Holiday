using System;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.Controls.Common;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatChannel : ChatChannelBase
    {
        public IObservable<string> OnMessageReceived
            => VivoxClient.OnTextMessageReceived.Select(channelTextMessage => channelTextMessage.Message);

        public TextChatChannel(VivoxClient vivoxClient, string channelName) : base(vivoxClient, channelName)
        {
        }

        protected override VivoxChannelConfig CreateChannelConfig(string channelName)
            => new VivoxChannelConfig(channelName, ChatType.TextOnly, transmissionSwitch: false);

        public void SendMessage(string message) => VivoxClient.SendTextMessage(message, ChannelId);
    }
}
