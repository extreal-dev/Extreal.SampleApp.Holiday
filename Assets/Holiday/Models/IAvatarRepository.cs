using System.Collections.Generic;

namespace Extreal.SampleApp.Holiday.Models
{
    public interface IAvatarRepository
    {
        List<Avatar> Avatars { get; }
    }
}
