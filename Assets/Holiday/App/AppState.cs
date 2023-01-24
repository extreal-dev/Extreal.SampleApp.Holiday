using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Common;
using UniRx;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppState : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppState));

        public IReadOnlyReactiveProperty<string> PlayerName => playerName;
        [SuppressMessage("Usage", "CC0033")]
        private readonly ReactiveProperty<string> playerName = new ReactiveProperty<string>("Guest");

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        [SuppressMessage("Usage", "CC0033")]
        private readonly ReactiveProperty<Avatar> avatar = new ReactiveProperty<Avatar>();

        public IObservable<bool> IsPlaying => isPlaying;
        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty isPlaying = new BoolReactiveProperty(false);

        public IObservable<string> OnNotificationReceived => onNotificationReceived;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onNotificationReceived = new Subject<string>();

        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty inMultiplay = new BoolReactiveProperty(false);
        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty inText = new BoolReactiveProperty(false);
        [SuppressMessage("Usage", "CC0033")]
        private readonly BoolReactiveProperty inAudio = new BoolReactiveProperty(false);

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public AppState()
        {
            inMultiplay.Merge(inText, inAudio)
                .Where(_ =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug(
                            $"inMultiplay: {inMultiplay.Value}, inText: {inText.Value}, inAudio: {inAudio.Value}");
                    }

                    return inMultiplay.Value && inText.Value && inAudio.Value;
                })
                .Subscribe(_ =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug("IsPlaying: true");
                    }

                    isPlaying.Value = true;
                })
                .AddTo(disposables);

            inMultiplay.Merge(inText, inAudio)
                .Where(_ =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug(
                            $"inMultiplay: {inMultiplay.Value}, inText: {inText.Value}, inAudio: {inAudio.Value}");
                    }

                    return !inMultiplay.Value && !inText.Value && !inAudio.Value;
                })
                .Subscribe(_ =>
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug("IsPlaying: false");
                    }

                    isPlaying.Value = false;
                })
                .AddTo(disposables);
        }

        public void SetPlayerName(string playerName) => this.playerName.Value = playerName;
        public void SetAvatar(Avatar avatar) => this.avatar.Value = avatar;
        public void SetInMultiplay(bool value) => inMultiplay.Value = value;
        public void SetInText(bool value) => inText.Value = value;
        public void SetInAudio(bool value) => inAudio.Value = value;

        public void SetNotification(string message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"OnNotificationReceived: {message}");
            }

            onNotificationReceived.OnNext(message);
        }

        protected override void FreeManagedResources()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(nameof(FreeManagedResources));
            }

            playerName.Dispose();
            avatar.Dispose();
            inMultiplay.Dispose();
            inText.Dispose();
            inAudio.Dispose();
            isPlaying.Dispose();
            onNotificationReceived.Dispose();
            disposables.Dispose();
        }
    }
}
