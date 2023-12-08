using System;
using Unity.Netcode;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.P2P
{
    [Serializable]
    public struct Message : INetworkSerializable
    {
        public readonly MessageId MessageId => messageId;
        [SerializeField] private MessageId messageId;

        public readonly INetworkSerializable Content => content;
        private INetworkSerializable content;

        [SerializeField] private string contentJson;
        [SerializeField] private string contentType;

        public Message(MessageId messageId, INetworkSerializable content)
        {
            this.messageId = messageId;
            this.content = content;
            contentType = this.content != null ? content.GetType().ToString() : default;
            contentJson = JsonUtility.ToJson(content);
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

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(contentType))
            {
                content = JsonUtility.FromJson(contentJson, Type.GetType(contentType)) as INetworkSerializable;
            }
        }
    }
}
