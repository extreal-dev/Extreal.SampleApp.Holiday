using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Avatars
{
    public class AvatarProvider : MonoBehaviour
    {
        [SerializeField] private Avatar avatar;

        public Avatar Avatar => avatar;
    }
}
