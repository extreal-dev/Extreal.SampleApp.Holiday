using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models
{
    public class AppState : IDisposable
    {
        public IReadOnlyReactiveProperty<string> PlayerName => playerName;
        private readonly ReactiveProperty<string> playerName = new ReactiveProperty<string>();

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        private readonly ReactiveProperty<Avatar> avatar = new ReactiveProperty<Avatar>();

        public IObservable<bool> IsPlaying => isPlaying;
        private readonly BoolReactiveProperty isPlaying = new BoolReactiveProperty(false);

        public List<Avatar> Avatars { get; private set; }

        public AppState(IAvatarRepository avatarRepository)
        {
            Avatars = avatarRepository.Avatars;
            playerName.Value = "Guest";
            avatar.Value = Avatars.First();
        }

        public void Dispose()
        {
            playerName.Dispose();
            avatar.Dispose();
            isPlaying.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SetPlayerName(string playerName)
            => this.playerName.Value = playerName;

        public void SetAvatar(string avatarName)
            => avatar.Value = Avatars.Find(a => a.Name == avatarName);

        public void SetIsPlaying(bool value)
            => isPlaying.Value = value;
    }
}
