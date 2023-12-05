using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class MultiplayCanvasControllerInput : MonoBehaviour
    {
        public HolidayPlayerInput HolidayPlayerInput => holidayPlayerInput;
        [SerializeField] private HolidayPlayerInput holidayPlayerInput;

        public void VirtualMoveInput(Vector2 virtualMoveDirection) => holidayPlayerInput.MoveInput(virtualMoveDirection);

        public void VirtualLookInput(Vector2 virtualLookDirection) => holidayPlayerInput.LookInput(virtualLookDirection);

        public void VirtualJumpInput(bool virtualJumpState) => holidayPlayerInput.JumpInput(virtualJumpState);

        public void VirtualSprintInput(bool virtualSprintState) => holidayPlayerInput.SprintInput(virtualSprintState);

        public void SetHolidayPlayerInput(HolidayPlayerInput holidayPlayerInput) => this.holidayPlayerInput = holidayPlayerInput;
    }
}
