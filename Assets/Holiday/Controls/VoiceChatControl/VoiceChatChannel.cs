using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatChannel : ChatChannelBase
    {
        public IObservable<bool> OnMuted => onMuted.AddTo(Disposables);
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly ReactiveProperty<bool> onMuted = new ReactiveProperty<bool>(true);

        private readonly VivoxClient vivoxClient;

        public VoiceChatChannel(VivoxClient vivoxClient, string channelName) : base(vivoxClient, channelName)
            => this.vivoxClient = vivoxClient;

        protected override void Connect(string channelName)
            => vivoxClient.Connect(new VivoxChannelConfig(channelName, ChatType.AudioOnly));

        public async UniTask ToggleMuteAsync()
        {
            var audioInputDevices = await vivoxClient.GetAudioInputDevicesAsync();
            onMuted.Value = !onMuted.Value;
            audioInputDevices.Muted = onMuted.Value;
        }
    }
}
