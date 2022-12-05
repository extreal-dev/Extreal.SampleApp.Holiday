using System.Collections.Generic;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models.ScriptableObject
{
    [CreateAssetMenu(
        menuName = "BuiltinRepository/" + nameof(BuiltinAvatarRepository),
        fileName = nameof(BuiltinAvatarRepository))]
    public class BuiltinAvatarRepository : UnityEngine.ScriptableObject, IAvatarRepository
    {
        [SerializeField] private List<Avatar> avatars;

        public List<Avatar> Avatars => avatars;
    }
}
