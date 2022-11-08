namespace Extreal.SampleApp.Holiday.Models
{
    using System.Collections.Generic;

    public interface IAvatarRepository
    {
        List<Avatar> Avatars { get; }
    }
}
