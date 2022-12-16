using System.Collections.Generic;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(BuiltinAvatarRepository),
        fileName = nameof(BuiltinAvatarRepository))]
    public class BuiltinAvatarRepository : ScriptableObject, IAvatarRepository
    {
        [SerializeField] private List<Avatar> avatars;

        public List<Avatar> Avatars => avatars;
    }
}
