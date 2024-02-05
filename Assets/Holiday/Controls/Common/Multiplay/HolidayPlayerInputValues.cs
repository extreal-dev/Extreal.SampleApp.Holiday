using System.Diagnostics.CodeAnalysis;
using Extreal.Integration.Multiplay.Messaging;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class HolidayPlayerInputValues : PlayerInputValues
    {
        private Vector2 preMove;
        private bool isMoveChanged;

        [SuppressMessage("Usage", "CC0047")] public bool Sprint { get; set; }
        private bool preSprint;
        private bool isSprintChanged;

        [SuppressMessage("Usage", "CC0047")] public bool Jump { get; set; }
        private bool preJump;
        private bool isJumpChanged;

        [SuppressMessage("Usage", "CC0047")] public bool InputFieldTyping { get; set; }
        private bool preInputFieldTyping;
        private bool isInputFieldTypingChanged;

        public override void SetMove(Vector2 move)
        {
            preMove = Move;
            base.SetMove(move);
            isMoveChanged = preMove != Move;
        }

        public void SetSprint(bool sprint)
        {
            preSprint = Sprint;
            Sprint = sprint;
            isSprintChanged = preSprint != Sprint;
        }

        public void SetJump(bool jump)
        {
            preJump = Jump;
            Jump = jump;
            isJumpChanged = preJump != Jump;
        }

        public void SetInputFieldTyping(bool inputFieldTyping)
        {
            preInputFieldTyping = InputFieldTyping;
            InputFieldTyping = inputFieldTyping;
            isInputFieldTypingChanged = preInputFieldTyping != InputFieldTyping;
        }

        public override bool CheckWhetherToSendData()
        {
            var ret = isMoveChanged || isSprintChanged || isJumpChanged || isInputFieldTypingChanged;
            isMoveChanged = isSprintChanged = isJumpChanged = isInputFieldTypingChanged = false;
            return ret;
        }
    }
}
