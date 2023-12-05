using Extreal.Integration.Multiplay.LiveKit;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class HolidayPlayerInput : RedisPlayerInput
    {
        public override MultiplayPlayerInputValues Values => HolidayValues;
        public HolidayPlayerInputValues HolidayValues { get; } = new HolidayPlayerInputValues();

        public override void SetMove(Vector2 newMoveDirection)
            => HolidayValues.SetMove(newMoveDirection);

        public void SetLook(Vector2 newLookDirection)
            => HolidayValues.SetLook(newLookDirection);

        public void SetSprint(bool newSprint)
            => HolidayValues.SetSprint(newSprint);

        public void SetJump(bool newJump)
            => HolidayValues.SetJump(newJump);

        public void SetMouseLeftButtonPressed(bool newValue)
            => HolidayValues.SetMouseLeftButtonPressed(newValue);

        public void SetInputFieldTyping(bool newValue)
            => HolidayValues.SetInputFieldTyping(newValue);

        public override void SetValues(MultiplayPlayerInputValues values)
        {
            var holidayValues = values as HolidayPlayerInputValues;

            base.SetValues(holidayValues);
            SetLook(holidayValues.Look);
            SetSprint(holidayValues.Sprint);
            SetJump(holidayValues.Jump);
            SetMouseLeftButtonPressed(holidayValues.MouseLeftButtonPressed);
            SetInputFieldTyping(holidayValues.InputFieldTyping);
        }
    }
}
