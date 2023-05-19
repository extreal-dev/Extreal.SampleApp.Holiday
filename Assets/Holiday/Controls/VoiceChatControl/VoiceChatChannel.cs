using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.Controls.Common;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatChannel : ChatChannelBase
    {
        public IObservable<bool> OnMuted => onMuted.AddTo(Disposables);
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly ReactiveProperty<bool> onMuted = new ReactiveProperty<bool>(true);

        public VoiceChatChannel(VivoxClient vivoxClient, string channelName) : base(vivoxClient, channelName)
            => SetMuteAsync(true).Forget();

        protected override VivoxChannelConfig CreateChannelConfig(string channelName)
            => new VivoxChannelConfig(channelName, ChatType.AudioOnly);

        public UniTaskVoid ToggleMuteAsync() => SetMuteAsync(!onMuted.Value);

        private async UniTaskVoid SetMuteAsync(bool muted)
        {
            var audioInputDevices = await VivoxClient.GetAudioInputDevicesAsync();
            audioInputDevices.Muted = muted;
            onMuted.Value = muted;
        }
    }
}
