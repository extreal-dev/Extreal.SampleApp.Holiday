using System;
using Extreal.Integration.Multiplay.LiveKit;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    [Serializable]
    public class HolidayPlayerInputValues : MultiplayPlayerInputValues
    {
        public bool Sprint => sprint;
        [SerializeField] private bool sprint;

        public bool Jump => jump;
        [SerializeField] private bool jump;

        public void SetSprint(bool sprint)
            => this.sprint = sprint;

        public void SetJump(bool jump)
            => this.jump = jump;
    }
}