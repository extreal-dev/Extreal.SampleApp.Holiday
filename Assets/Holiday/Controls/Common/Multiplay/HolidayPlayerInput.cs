using Extreal.Integration.Multiplay.LiveKit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class HolidayPlayerInput : RedisPlayerInput
    {
        public override MultiplayPlayerInputValues Values => HolidayValues;
        public HolidayPlayerInputValues HolidayValues { get; } = new HolidayPlayerInputValues();

        public Vector2 Look => look;
        private Vector2 look;

        public void OnMove(InputValue value)
            => MoveInput(value.Get<Vector2>());

        public void OnLook(InputValue value)
            => LookInput(value.Get<Vector2>());

        public void OnSprint(InputValue value)
            => SprintInput(value.isPressed);

        public void OnJump(InputValue value)
            => JumpInput(value.isPressed);

        public void LookInput(Vector2 newLookDirection)
            => look = newLookDirection;

        public void SprintInput(bool newSprint)
            => HolidayValues.SetSprint(newSprint);

        public void JumpInput(bool newJump)
            => HolidayValues.SetJump(newJump);

        public override void SetValues(MultiplayPlayerInputValues values)
        {
            var applicationValues = values as HolidayPlayerInputValues;

            base.SetValues(applicationValues);
            SprintInput(applicationValues.Sprint);
            JumpInput(applicationValues.Jump);
        }
    }
}