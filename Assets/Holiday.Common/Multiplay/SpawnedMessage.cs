using Unity.Netcode;

namespace Extreal.SampleApp.Holiday.Common.Multiplay
{
    public struct SpawnedMessage : INetworkSerializable
    {
        public ulong NetworkObjectId => networkObjectId;
        public string AvatarAssetName => avatarAssetName;

        private ulong networkObjectId;
        private string avatarAssetName;

        public SpawnedMessage(ulong networkObjectId, string avatarAssetName)
        {
            this.networkObjectId = networkObjectId;
            this.avatarAssetName = avatarAssetName;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref networkObjectId);
            serializer.SerializeValue(ref avatarAssetName);
        }
    }
}
