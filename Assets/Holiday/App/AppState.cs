﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Group;
using Extreal.SampleApp.Holiday.Controls.RetryStatusControl;
using Extreal.SampleApp.Holiday.Screens.ConfirmationScreen;
using UniRx;
using Message = Extreal.SampleApp.Holiday.App.P2P.Message;

namespace Extreal.SampleApp.Holiday.App
{
    [SuppressMessage("Usage", "CC0033")]
    public class AppState : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppState));

        private GroupRole role = GroupRole.Host;

        public string PlayerName { get; private set; } = "Guest";
        public AvatarConfig.Avatar Avatar { get; private set; }
        public SpaceConfig.Space Space { get; private set; }
        public bool IsHost => role == GroupRole.Host;
        public bool IsClient => role == GroupRole.Client;
        public string GroupName { get; private set; }
        public string SpaceName { get; private set; }

        public IReadOnlyReactiveProperty<bool> PlayingReady => playingReady.AddTo(disposables);
        private readonly ReactiveProperty<bool> playingReady = new ReactiveProperty<bool>(false);

        public IReadOnlyReactiveProperty<bool> SpaceReady => spaceReady.AddTo(disposables);
        private readonly ReactiveProperty<bool> spaceReady = new ReactiveProperty<bool>(false);

        public IReadOnlyReactiveProperty<bool> SfuReady => sfuReady.AddTo(disposables);
        private readonly ReactiveProperty<bool> sfuReady = new ReactiveProperty<bool>(false);

        public IObservable<Message> OnMessageSent => onMessageSent.AddTo(disposables);
        private readonly Subject<Message> onMessageSent = new Subject<Message>();

        public IObservable<Message> OnMessageReceived => onMessageReceived.AddTo(disposables);
        private readonly Subject<Message> onMessageReceived = new Subject<Message>();

        public IObservable<string> OnNotificationReceived => onNotificationReceived.AddTo(disposables);
        private readonly Subject<string> onNotificationReceived = new Subject<string>();

        public IObservable<Confirmation> OnConfirmationReceived => onConfirmationReceived.AddTo(disposables);
        private readonly Subject<Confirmation> onConfirmationReceived = new Subject<Confirmation>();

        public IObservable<RetryStatus> OnRetryStatusReceived => onRetryStatusReceived.AddTo(disposables);
        private readonly Subject<RetryStatus> onRetryStatusReceived = new Subject<RetryStatus>();

        private readonly BoolReactiveProperty multiplayReady = new BoolReactiveProperty(false);
        private readonly BoolReactiveProperty landscapeInitialized = new BoolReactiveProperty(false);

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public StageState StageState { get; private set; }

        public AppState()
        {
            multiplayReady.AddTo(disposables);
            landscapeInitialized.AddTo(disposables);

            MonitorPlayingReadyStatus();
            RestorePlayingReadyStatus();
        }

        [SuppressMessage("Usage", "CC0033")]
        private void MonitorPlayingReadyStatus() =>
            multiplayReady.Merge(sfuReady, spaceReady, landscapeInitialized)
                .Where(_ =>
                {
                    LogWaitingStatus();
                    return multiplayReady.Value && sfuReady.Value && spaceReady.Value && landscapeInitialized.Value;
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

        [SuppressMessage("Usage", "CC0033")]
        private void RestorePlayingReadyStatus() =>
            spaceReady.Merge(landscapeInitialized)
                .Where(_ =>
                {
                    LogWaitingStatus();
                    return !spaceReady.Value && !landscapeInitialized.Value;
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
                Logger.LogDebug($"Multiplay, SFU, Space Ready, Landscape Initialized: " +
                                $"{multiplayReady.Value}, {sfuReady.Value}, {spaceReady.Value}, {landscapeInitialized.Value}");
            }
        }

        public void SetPlayerName(string playerName) => PlayerName = playerName;
        public void SetAvatar(AvatarConfig.Avatar avatar) => Avatar = avatar;
        public void SetSpace(SpaceConfig.Space space) => Space = space;
        public void SetRole(GroupRole role) => this.role = role;
        public void SetGroupName(string groupName) => GroupName = groupName;
        public void SetSpaceName(string spaceName) => SpaceName = spaceName;
        public void SetMultiplayReady(bool ready) => multiplayReady.Value = ready;
        public void SetSfuReady(bool ready) => sfuReady.Value = ready;
        public void SetSpaceReady(bool ready) => spaceReady.Value = ready;
        public void SetLandscapeInitialized(bool initialized) => landscapeInitialized.Value = initialized;
        public void SetStage(StageName stageName) => StageState = new StageState(stageName);

        public void SendMessage(Message message) => onMessageSent.OnNext(message);

        public void ReceivedMessage(Message message) => onMessageReceived.OnNext(message);

        public void Notify(string message)
        {
            Logger.LogError(message);
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

        public void Retry(RetryStatus retryStatus)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Retry status received: {retryStatus.State} {retryStatus.Message}");
            }
            onRetryStatusReceived.OnNext(retryStatus);
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
