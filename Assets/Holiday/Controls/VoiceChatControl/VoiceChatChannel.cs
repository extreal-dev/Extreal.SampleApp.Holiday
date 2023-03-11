using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.Controls.Common;
using UniRx;
using VivoxUnity;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatChannel : ChatChannelBase
    {
        public IObservable<bool> OnMuted => onMuted.AddTo(Disposables);
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly ReactiveProperty<bool> onMuted = new ReactiveProperty<bool>();

        private readonly VivoxClient vivoxClient;

        public VoiceChatChannel(VivoxClient vivoxClient, string channelName) : base(vivoxClient, channelName)
        {
            this.vivoxClient = vivoxClient;
            SetMuteAsync(true).Forget();
        }

        protected override UniTask<ChannelId> ConnectAsync(string channelName)
            => vivoxClient.ConnectAsync(new VivoxChannelConfig(channelName, ChatType.AudioOnly));

        public UniTask ToggleMuteAsync() => SetMuteAsync(!onMuted.Value);

        private async UniTask SetMuteAsync(bool muted)
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted = muted;
            onMuted.Value = muted;
        }
    }
}
