using Extreal.SampleApp.Holiday.App.Config;
using Unity.Netcode;

namespace Extreal.SampleApp.Holiday.Controls.SpaceControl
{
    public struct SpaceTransitionMessageContent : INetworkSerializable
    {
        public readonly StageName StageName => stageName;
        private StageName stageName;

        public SpaceTransitionMessageContent(StageName stageName)
            => this.stageName = stageName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            => serializer.SerializeValue(ref stageName);
    }
}
