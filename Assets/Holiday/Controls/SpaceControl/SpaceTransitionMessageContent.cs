using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.P2P;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.SpaceControl
{
    [Serializable]
    public struct SpaceTransitionMessageContent : IMessageContent
    {
        public readonly StageName StageName => stageName;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private StageName stageName;

        public SpaceTransitionMessageContent(StageName stageName)
            => this.stageName = stageName;
    }
}
