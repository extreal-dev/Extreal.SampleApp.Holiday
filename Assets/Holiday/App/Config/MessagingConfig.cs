using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Integration.Messaging.Socket.IO;
using SocketIOClient;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(MessagingConfig),
        fileName = nameof(MessagingConfig))]
    public class MessagingConfig : ScriptableObject
    {
        public SocketIOMessagingConfig SocketIOMessagingConfig
            => new SocketIOMessagingConfig(
                messagingUrl,
                new SocketIOOptions
                {
                    ConnectionTimeout = TimeSpan.FromSeconds(timeoutSeconds),
                    Reconnection = false,
                });
        [SerializeField, SuppressMessage("Usage", "CC0052")] private string messagingUrl = "http://127.0.0.1:3030";
        [SerializeField, SuppressMessage("Usage", "CC0052")] private int timeoutSeconds = 3;
    }
}
