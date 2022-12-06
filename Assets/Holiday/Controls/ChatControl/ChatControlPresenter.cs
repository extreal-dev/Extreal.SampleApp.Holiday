using System;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.Models;
using UniRx;
using VContainer.Unity;
using Extreal.Core.StageNavigation;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class ChatControlPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly VivoxClient vivoxClient;
        private readonly TextChatChannel textChatChannel;
        private readonly VoiceChatChannel voiceChatChannel;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public ChatControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VivoxClient vivoxClient,
            TextChatChannel textChatChannel,
            VoiceChatChannel voiceChatChannel,
            AppState appState
        )
        {
            this.stageNavigator = stageNavigator;
            this.vivoxClient = vivoxClient;
            this.textChatChannel = textChatChannel;
            this.voiceChatChannel = voiceChatChannel;
            this.appState = appState;
        }

        public void Initialize()
        {
            stageNavigator.OnStageTransitioned
                .Subscribe(OnStageEntered)
                .AddTo(disposables);

            stageNavigator.OnStageTransitioning
                .Subscribe(OnStageExiting)
                .AddTo(disposables);

            textChatChannel.InText
                .Subscribe(appState.SetInText)
                .AddTo(disposables);

            voiceChatChannel.InAudio
                .Subscribe(appState.SetInAudio)
                .AddTo(disposables);

            appState.InMultiplay
                .Where(inMultiplay => inMultiplay)
                .Subscribe(_ =>
                {
                    var authConfig = new VivoxAuthConfig(appState.PlayerName.Value);
                    vivoxClient.Login(authConfig);
                })
                .AddTo(disposables);
        }

        public void Dispose()
        {
            vivoxClient.Logout();

            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageEntered(StageName stageName)
        {
            textChatChannel.Join($"HolidayTextChat{stageName}");
            voiceChatChannel.Join($"HolidayVoiceChat{stageName}");
        }

        private void OnStageExiting(StageName stageName)
        {
            textChatChannel.Leave();
            voiceChatChannel.Leave();
        }
    }
}
