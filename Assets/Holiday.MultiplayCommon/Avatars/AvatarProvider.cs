using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayCommon
{
    public class AvatarProvider : MonoBehaviour
    {
        [SerializeField] private Avatar avatar;

        public Avatar Avatar => avatar;
    }
}
