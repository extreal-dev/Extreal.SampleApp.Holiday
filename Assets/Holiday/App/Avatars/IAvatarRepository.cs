using System.Collections.Generic;

namespace Extreal.SampleApp.Holiday.App.Avatars
{
    public interface IAvatarRepository
    {
        List<Avatar> Avatars { get; }
    }
}
