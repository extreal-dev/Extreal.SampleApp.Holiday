﻿using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.Screens.ConfirmationScreen;
using UniRx;

namespace Extreal.SampleApp.Holiday.App
{
    [SuppressMessage("Usage", "CC0033")]
    public class AppState : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppState));

        public IReadOnlyReactiveProperty<string> PlayerName => playerName.AddTo(disposables);
        private readonly ReactiveProperty<string> playerName = new ReactiveProperty<string>("Guest");

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar.AddTo(disposables);
        private readonly ReactiveProperty<Avatar> avatar = new ReactiveProperty<Avatar>(null);

        public IReadOnlyReactiveProperty<string> SpaceName => spaceName.AddTo(disposables);
        private readonly ReactiveProperty<string> spaceName = new ReactiveProperty<string>(null);

        public IReadOnlyReactiveProperty<bool> PlayingReady => playingReady.AddTo(disposables);
        private readonly ReactiveProperty<bool> playingReady = new ReactiveProperty<bool>(false);

        public IReadOnlyReactiveProperty<bool> SpaceReady => spaceReady.AddTo(disposables);
        private readonly ReactiveProperty<bool> spaceReady = new ReactiveProperty<bool>(false);

        public IObservable<string> OnNotificationReceived => onNotificationReceived.AddTo(disposables);
        private readonly Subject<string> onNotificationReceived = new Subject<string>();

        public IObservable<Confirmation> OnConfirmationReceived => onConfirmationReceived.AddTo(disposables);
        private readonly Subject<Confirmation> onConfirmationReceived = new Subject<Confirmation>();

        private readonly BoolReactiveProperty multiplayReady = new BoolReactiveProperty(false);
        private readonly BoolReactiveProperty textChatReady = new BoolReactiveProperty(false);
        private readonly BoolReactiveProperty voiceChatReady = new BoolReactiveProperty(false);

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public AppState()
        {
            multiplayReady.AddTo(disposables);
            textChatReady.AddTo(disposables);
            voiceChatReady.AddTo(disposables);

            MonitorPlayingReadyStatus();
            RestorePlayingReadyStatus();
        }

        private void MonitorPlayingReadyStatus() =>
            multiplayReady.Merge(textChatReady, voiceChatReady, spaceReady)
                .Where(_ =>
                {
                    LogWaitingStatus();
                    return multiplayReady.Value && textChatReady.Value && voiceChatReady.Value && spaceReady.Value;
                })
                .Subscribe(_ =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug("Ready to play");
                    }
                    playingReady.Value = true;
                })
                .AddTo(disposables);

        private void RestorePlayingReadyStatus() =>
            multiplayReady.Merge(textChatReady, voiceChatReady, spaceReady)
                .Where(_ =>
                {
                    LogWaitingStatus();
                    return !multiplayReady.Value && !textChatReady.Value && !voiceChatReady.Value && !spaceReady.Value;
                })
                .Subscribe(_ =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug("Stop playing");
                    }
                    playingReady.Value = false;
                })
                .AddTo(disposables);

        private void LogWaitingStatus()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Multiplay, TextChat, VoiceChat, Space Ready: " +
                                $"{multiplayReady.Value}, {textChatReady.Value}, " +
                                $"{voiceChatReady.Value}, {spaceReady.Value}");
            }
        }

        public void SetPlayerName(string playerName) => this.playerName.Value = playerName;
        public void SetAvatar(Avatar avatar) => this.avatar.Value = avatar;
        public void SetMultiplayReady(bool ready) => multiplayReady.Value = ready;
        public void SetTextChatReady(bool ready) => textChatReady.Value = ready;
        public void SetVoiceChatReady(bool ready) => voiceChatReady.Value = ready;
        public void SetSpaceReady(bool ready) => spaceReady.Value = ready;

        public void Notify(string message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Notification received: {message}");
            }
            onNotificationReceived.OnNext(message);
        }

        public void Confirm(Confirmation confirmation)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Confirmation received: {confirmation.Message}");
            }
            onConfirmationReceived.OnNext(confirmation);
        }

        public void LoadSpace(string spaceName)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Space name received: {spaceName}");
            }
            this.spaceName.Value = spaceName;
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
