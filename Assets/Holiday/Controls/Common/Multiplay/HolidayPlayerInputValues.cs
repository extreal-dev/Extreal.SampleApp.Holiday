using System;
using Extreal.Integration.Multiplay.LiveKit;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    [Serializable]
    public class HolidayPlayerInputValues : MultiplayPlayerInputValues
    {
        public Vector2 Look => look;
        [SerializeField] private Vector2 look;

        public bool Sprint => sprint;
        [SerializeField] private bool sprint;

        public bool Jump => jump;
        [SerializeField] private bool jump;

        public bool MouseLeftButtonPressed => mouseLeftButtonPressed;
        [SerializeField] private bool mouseLeftButtonPressed;

        public bool InputFieldTyping => inputFieldTyping;
        [SerializeField] private bool inputFieldTyping;


        public void SetLook(Vector2 look)
            => this.look = look;

        public void SetSprint(bool sprint)
            => this.sprint = sprint;

        public void SetJump(bool jump)
            => this.jump = jump;

        public void SetMouseLeftButtonPressed(bool mouseLeftButtonPressed)
            => this.mouseLeftButtonPressed = mouseLeftButtonPressed;

        public void SetInputFieldTyping(bool inputFieldTyping)
            => this.inputFieldTyping = inputFieldTyping;
    }
}
