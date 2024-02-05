using Extreal.Integration.Multiplay.Messaging;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class HolidayPlayerInput : PlayerInput
    {
        public override PlayerInputValues Values => HolidayValues;
        public HolidayPlayerInputValues HolidayValues { get; } = new HolidayPlayerInputValues();

        public Vector2 Look { get; private set; }

        public void SetLook(Vector2 newLookDirection)
            => Look = newLookDirection;

        public void SetSprint(bool newSprint)
            => HolidayValues.SetSprint(newSprint);

        public void SetJump(bool newJump)
            => HolidayValues.SetJump(newJump);

        public void SetInputFieldTyping(bool newValue)
            => HolidayValues.SetInputFieldTyping(newValue);

        public override void ApplyValues(PlayerInputValues synchronizedValues)
        {
            var synchronizedHolidayValues = synchronizedValues as HolidayPlayerInputValues;

            base.ApplyValues(synchronizedHolidayValues);
            SetSprint(synchronizedHolidayValues.Sprint);
            SetJump(synchronizedHolidayValues.Jump);
            SetInputFieldTyping(synchronizedHolidayValues.InputFieldTyping);
        }
    }
}
