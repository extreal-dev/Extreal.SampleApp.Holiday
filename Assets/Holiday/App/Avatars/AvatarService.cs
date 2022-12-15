using System.Linq;

namespace Extreal.SampleApp.Holiday.App
{
    public class AvatarService
    {
        public Avatar[] Avatars { get; }

        public AvatarService(IAvatarRepository avatarRepository)
            => Avatars = avatarRepository.Avatars.ToArray();

        public Avatar FindAvatarByName(string name)
            => Avatars.First(avatar => avatar.Name == name);
    }
}
