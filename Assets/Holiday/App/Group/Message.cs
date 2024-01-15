using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.P2P
{
    [Serializable]
    public class Message : ISerializationCallbackReceiver
    {
        public MessageId MessageId => messageId;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private MessageId messageId;

        public IMessageContent Content { get; private set; }

        [SerializeField] private string contentType;
        [SerializeField] private string contentJson;

        public Message(MessageId messageId, IMessageContent content)
        {
            this.messageId = messageId;
            Content = content;
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(contentType))
            {
                Content = JsonUtility.FromJson(contentJson, Type.GetType(contentType)) as IMessageContent;
            }
        }

        public void OnBeforeSerialize()
        {
            contentType = Content != null ? Content.GetType().ToString() : default;
            contentJson = JsonUtility.ToJson(Content);
        }
    }
}
