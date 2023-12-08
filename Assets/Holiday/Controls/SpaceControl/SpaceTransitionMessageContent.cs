using System;
using Extreal.SampleApp.Holiday.App.Config;
using Unity.Netcode;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.SpaceControl
{
    [Serializable]
    public struct SpaceTransitionMessageContent : INetworkSerializable
    {
        public readonly StageName StageName => stageName;
        [SerializeField] private StageName stageName;

        public SpaceTransitionMessageContent(StageName stageName)
            => this.stageName = stageName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            => serializer.SerializeValue(ref stageName);
    }
}
