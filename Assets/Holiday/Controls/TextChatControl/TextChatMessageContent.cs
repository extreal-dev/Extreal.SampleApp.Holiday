using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.P2P;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatMessageContent : IMessageContent
    {
        public string MessageContent => messageContent;
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string messageContent;

        public TextChatMessageContent(string messageContent)
            => this.messageContent = messageContent;
    }
}
