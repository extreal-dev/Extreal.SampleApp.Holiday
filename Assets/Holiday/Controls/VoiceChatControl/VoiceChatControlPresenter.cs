using System;
using Cysharp.Threading.Tasks;
using VContainer.Unity;
using UniRx;
using Extreal.SampleApp.Holiday.Models;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly VoiceChatControlView voiceChatScreenView;
        private readonly VoiceChatChannel voiceChatChannel;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public VoiceChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VoiceChatControlView voiceChatScreenView,
            VoiceChatChannel voiceChatChannel,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.voiceChatScreenView = voiceChatScreenView;
            this.voiceChatChannel = voiceChatChannel;
            this.appState = appState;
        }

        public void Initialize()
        {
            stageNavigator.OnStageTransitioned
                .Subscribe(stageName => voiceChatChannel.SetChannelName($"HolidayVoiceChat{stageName}"))
                .AddTo(disposables);

            stageNavigator.OnStageTransitioning
                .Subscribe(_ => voiceChatChannel.Leave())
                .AddTo(disposables);

            voiceChatChannel.InAudio
                .Subscribe(appState.SetInAudio)
                .AddTo(disposables);

            voiceChatScreenView.OnMuteButtonClicked
                .Subscribe(_ => voiceChatChannel.ToggleMuteAsync().Forget())
                .AddTo(disposables);

            voiceChatChannel.OnMuted
                .Subscribe(voiceChatScreenView.SetMutedString)
                .AddTo(disposables);

            appState.InMultiplay
            .Where(inMultiplay => inMultiplay)
            .Subscribe(_ => voiceChatChannel.Join())
            .AddTo(disposables);

            voiceChatChannel.Login();
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
