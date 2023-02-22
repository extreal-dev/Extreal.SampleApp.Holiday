using System.Collections.Generic;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Avatars
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(AvatarRepository),
        fileName = nameof(AvatarRepository))]
    public class AvatarRepository : ScriptableObject
    {
        [SerializeField] private List<Avatar> avatars;

        public AvatarService ToAvatarService()
            => new AvatarService(avatars.ToArray());
    }
}
