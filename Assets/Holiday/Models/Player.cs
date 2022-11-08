namespace Extreal.SampleApp.Holiday.Models
{
    using UniRx;

    public class Player
    {
        private readonly ReactiveProperty<string> name = new("Guest");
        private readonly ReactiveProperty<AvatarName> avatar = new(AvatarName.Armature);

        public IReadOnlyReactiveProperty<string> Name => name;
        public IReadOnlyReactiveProperty<AvatarName> Avatar => avatar;

        public void SetName(string name) => this.name.Value = name;

        public void SetAvatar(AvatarName avatar) => this.avatar.Value = avatar;
    }
}
