namespace Extreal.SampleApp.Holiday.Models.ScriptableObject
{
    using System.Collections.Generic;
    using UnityEngine;
    using Avatar = Models.Avatar;

    [CreateAssetMenu(
        menuName = "BuiltinRepository/" + nameof(BuiltinAvatarRepository),
        fileName = nameof(BuiltinAvatarRepository))]
    public class BuiltinAvatarRepository : ScriptableObject, IAvatarRepository
    {
        [SerializeField] private List<Avatar> avatars;

        public List<Avatar> Avatars => avatars;
    }
}
