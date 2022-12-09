using System.Linq;

namespace Extreal.SampleApp.Holiday.App.Avatars
{
    public class AvatarService
    {
        public Avatar[] Avatars { get; private set; }

        public AvatarService(IAvatarRepository avatarRepository)
            => Avatars = avatarRepository.Avatars.ToArray();

        public Avatar FindAvatarByName(string name)
            => Avatars.First(avatar => avatar.Name == name);
    }
}
