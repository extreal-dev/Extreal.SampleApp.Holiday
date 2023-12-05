using UnityEngine;

namespace Extreal.SampleApp.Holiday
{
    public interface INetworkThirdPersonController
    {
        void Initialize(Avatar avatar, bool isOwner, bool isTouchDevice);
        void ResetPosition();
        void DoLateUpdate();
    }
}
