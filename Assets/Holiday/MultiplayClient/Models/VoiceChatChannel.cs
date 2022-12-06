using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Chat.Vivox;
using UniRx;
using VContainer.Unity;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models
{
    public class VoiceChatChannel : IInitializable, IDisposable
    {
        public IObservable<IParticipant> OnConnected
            => vivoxClient.OnUserConnected.Where(participant => participant.IsSelf && participant.InAudio);
        public IObservable<IParticipant> OnDisconnected
            => vivoxClient.OnUserDisconnected.Where(participant => participant.IsSelf && participant.InAudio);

        private readonly VivoxClient vivoxClient;

        private ChannelId channelId;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(TextChatChannel));

        public VoiceChatChannel(VivoxClient vivoxClient)
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
            var channelConfig = new VivoxChannelConfig("HolidayTextChat", ChatType.AudioOnly);
            vivoxClient.Connect(channelConfig);
        }

        public void Leave()
            => vivoxClient.Disconnect(channelId);

        public async UniTask ToggleMuteAsync()
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted ^= true;
        }
    }
}
