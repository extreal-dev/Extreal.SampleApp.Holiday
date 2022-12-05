using System.Collections.Generic;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models
{
    public interface IAvatarRepository
    {
        List<Avatar> Avatars { get; }
    }
}
