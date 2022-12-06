using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Chat.Vivox;
using UniRx;
using VContainer.Unity;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models
{
    public class TextChatChannel : IInitializable, IDisposable
    {
        public IObservable<IParticipant> OnConnected
            => vivoxClient.OnUserConnected.Where(participant => participant.IsSelf && participant.InText);
        public IObservable<IParticipant> OnDisconnected
            => vivoxClient.OnUserDisconnected.Where(participant => participant.IsSelf && participant.InText);
        public IObservable<string> OnTextMessageReceived
            => vivoxClient.OnTextMessageReceived.Select(channelTextMessage => channelTextMessage.Message);

        private readonly VivoxClient vivoxClient;

        private ChannelId channelId;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(TextChatChannel));

        public TextChatChannel(VivoxClient vivoxClient)
            => this.vivoxClient = vivoxClient;

        public void Initialize()
            => OnConnected
                .Subscribe(participant => channelId = participant.ParentChannelSession.Channel)
                .AddTo(disposables);

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Join()
        {
            var channelConfig = new VivoxChannelConfig("HolidayTextChat", ChatType.TextOnly, transmissionSwitch: false);
            vivoxClient.Connect(channelConfig);
        }

        public void Leave()
            => vivoxClient.Disconnect(channelId);

        public void SendTextMessage(string message)
            => vivoxClient.SendTextMessage(message, channelId);
    }
}
