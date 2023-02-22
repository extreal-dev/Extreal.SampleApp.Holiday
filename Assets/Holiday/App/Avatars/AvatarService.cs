using System.Linq;

namespace Extreal.SampleApp.Holiday.App.Avatars
{
    public class AvatarService
    {
        public Avatar[] Avatars { get; private set; }

        public AvatarService(Avatar[] avatars)
            => Avatars = avatars;

        public Avatar FindAvatarByName(string name)
            => Avatars.First(avatar => avatar.Name == name);
    }
}
