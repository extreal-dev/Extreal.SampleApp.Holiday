using System;
using System.Linq;
using Extreal.SampleApp.Holiday.Models;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App
{
    public class AppState : IInitializable, IDisposable
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
            isPlaying.Dispose();
            inMultiplay.Dispose();
            inText.Dispose();
            inAudio.Dispose();
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
    }
}
