using System;
using Unity.Netcode;

namespace Extreal.SampleApp.Holiday.App.P2P
{
    public struct Message : INetworkSerializable
    {
        public readonly MessageId MessageId => messageId;
        private MessageId messageId;

        public readonly INetworkSerializable Content => content;
        private INetworkSerializable content;

        public Message(MessageId messageId, INetworkSerializable content)
        {
            this.messageId = messageId;
            this.content = content;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var contentType = serializer.IsWriter ? content.GetType().ToString() : default;
            serializer.SerializeValue(ref contentType);
            if (serializer.IsReader)
            {
                content = Activator.CreateInstance(Type.GetType(contentType)) as INetworkSerializable;
            }

            serializer.SerializeValue(ref messageId);
            content.NetworkSerialize(serializer);
        }
    }
}
