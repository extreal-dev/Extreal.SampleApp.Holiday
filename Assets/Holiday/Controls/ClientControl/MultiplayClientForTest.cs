using System.Collections.Generic;
using Extreal.Integration.Messaging;
using Extreal.Integration.Multiplay.Messaging;
using Extreal.SampleApp.Holiday.Controls.Common.Multiplay;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class MultiplayClientForTest : MultiplayClient
    {
        public HashSet<string> UpdatedClients { get; } = new HashSet<string>();

        private readonly PlayerInput input;
        private readonly HolidayPlayerInput holidayInput;

        public MultiplayClientForTest(QueuingMessagingClient messagingClient, INetworkObjectsProvider networkObjectsProvider)
            : base(messagingClient, networkObjectsProvider)
            => input = holidayInput = new GameObject("InputForTest").AddComponent<HolidayPlayerInput>();

        protected override void ReleaseManagedResources()
        {
            Object.Destroy(input.gameObject);
            base.ReleaseManagedResources();
        }

        protected override void SynchronizeLocal()
        {
            while (MessagingClient.ResponseQueueCount() > 0)
            {
                (var from, var messageJson) = MessagingClient.DequeueResponse();
                var message = MultiplayMessage.FromJson(messageJson);
                if (message.Command == MultiplayMessageCommand.Update)
                {
                    message.NetworkObjectInfos[0].ApplyValuesTo(in input);
                    if (holidayInput.HolidayValues.Move != default)
                    {
                        UpdatedClients.Add(from);
                    }
                }
                else if (message.Command is MultiplayMessageCommand.UserInitialized)
                {
                    OnUserJoinedProtected.OnNext(from);
                }
            }
        }
    }
}
