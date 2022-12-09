using System.Collections.Generic;
using Extreal.SampleApp.Holiday.App.Avatars;
using UnityEngine;
using Avatar = Extreal.SampleApp.Holiday.App.Avatars.Avatar;

namespace Extreal.SampleApp.Holiday.App.BuiltinRepository
{
    [CreateAssetMenu(
        menuName = "BuiltinRepository/" + nameof(BuiltinAvatarRepository),
        fileName = nameof(BuiltinAvatarRepository))]
    public class BuiltinAvatarRepository : ScriptableObject, IAvatarRepository
    {
        [SerializeField] private List<Avatar> avatars;

        public List<Avatar> Avatars => avatars;
    }
}
