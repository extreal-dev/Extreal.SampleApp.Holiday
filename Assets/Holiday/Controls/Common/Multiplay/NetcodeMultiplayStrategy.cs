using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class NetcodeMultiplayStrategy : MultiplayStrategyBase
    {
        public NetcodeMultiplayStrategy(GameObject player, GameObject cinemachineCameraTarget, LayerMask groundLayers) : base(player, cinemachineCameraTarget, groundLayers)
        {
        }

        public override void DoLateUpdate()
        {
            if (isOwner)
            {
                GroundedCheck();

                if (!Input.HolidayValues.InputFieldTyping)
                {
                    JumpAndGravity(true);
                    Move(true);

                    if (!isTouchDevice)
                    {
                        MouseCameraRotation();
                    }
                    else
                    {
                        TouchDeviceCameraRotation();
                    }
                }
                else
                {
                    JumpAndGravity(false);
                    Move(false);
                }
            }
        }
    }
}
