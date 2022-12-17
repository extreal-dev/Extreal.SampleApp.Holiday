using System;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App.Avatars;
using UniRx;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppState : IDisposable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(AppState));

        public IReadOnlyReactiveProperty<string> PlayerName => playerName;
        private readonly ReactiveProperty<string> playerName = new ReactiveProperty<string>("Guest");

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        private readonly ReactiveProperty<Avatar> avatar = new ReactiveProperty<Avatar>();

        public IObservable<bool> IsPlaying => isPlaying;
        private readonly BoolReactiveProperty isPlaying = new BoolReactiveProperty(false);

        public IObservable<string> OnNotificationReceived => onNotificationReceived;
        private readonly Subject<string> onNotificationReceived = new Subject<string>();

        private readonly BoolReactiveProperty inMultiplay = new BoolReactiveProperty(false);
        private readonly BoolReactiveProperty inText = new BoolReactiveProperty(false);
        private readonly BoolReactiveProperty inAudio = new BoolReactiveProperty(false);

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

        public void Dispose()
        {
            playerName.Dispose();
            avatar.Dispose();
            inMultiplay.Dispose();
            inText.Dispose();
            inAudio.Dispose();
            isPlaying.Dispose();
            onNotificationReceived.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
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
    }
}
