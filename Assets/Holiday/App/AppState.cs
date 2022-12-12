using System;
using UniRx;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppState : IDisposable
    {
        public IReadOnlyReactiveProperty<string> PlayerName => playerName;
        private readonly ReactiveProperty<string> playerName = new ReactiveProperty<string>("Guest");

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        private readonly ReactiveProperty<Avatar> avatar = new ReactiveProperty<Avatar>();

        public IObservable<bool> InMultiplay => inMultiplay;
        private readonly BoolReactiveProperty inMultiplay = new BoolReactiveProperty(false);

        public IObservable<bool> InText => inText;
        private readonly BoolReactiveProperty inText = new BoolReactiveProperty(false);

        public IObservable<bool> InAudio => inAudio;
        private readonly BoolReactiveProperty inAudio = new BoolReactiveProperty(false);

        public IObservable<bool> IsPlaying => isPlaying;
        private readonly BoolReactiveProperty isPlaying = new BoolReactiveProperty(false);

        public IObservable<string> OnErrorOccurred => onErrorOccurred;
        private readonly Subject<string> onErrorOccurred = new Subject<string>();

        public bool IsErrorShowed { get; private set; }

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            InMultiplay.Merge(InText, InAudio)
                .Where(_ => inMultiplay.Value && inText.Value && inAudio.Value)
                .Subscribe(_ => isPlaying.Value = true)
                .AddTo(disposables);

            InMultiplay.Merge(InText, InAudio)
                .Where(value => isPlaying.Value && !value)
                .Subscribe(_ => isPlaying.Value = false)
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
            onErrorOccurred.Dispose();
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SetPlayerName(string playerName)
            => this.playerName.Value = playerName;

        public void SetAvatar(Avatar avatar)
            => this.avatar.Value = avatar;

        public void SetInMultiplay(bool value)
            => inMultiplay.Value = value;

        public void SetInText(bool value)
            => inText.Value = value;

        public void SetInAudio(bool value)
            => inAudio.Value = value;

        public void SetErrorMessage(string message)
            => onErrorOccurred.OnNext(message);

        public void SetIsErrorShowed(bool value)
            => IsErrorShowed = value;
    }
}
