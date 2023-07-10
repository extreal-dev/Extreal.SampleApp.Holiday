using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extreal.Core.Logging;
using Extreal.P2P.Dev;
using Unity.WebRTC;

namespace Extreal.Chat.Dev
{
    public class NativeTextChatClient : TextChatClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(NativeTextChatClient));

        private static readonly string Label = "textchat";

        private readonly Dictionary<string, List<RTCDataChannel>> dcDict;

        public NativeTextChatClient(NativePeerClient peerClient)
        {
            dcDict = new Dictionary<string, List<RTCDataChannel>>();
            peerClient.AddPcCreateHook(CreatePc);
            peerClient.AddPcCloseHook(ClosePc);
        }

        private void CreatePc(string id, bool isOffer, RTCPeerConnection pc)
        {
            if (isOffer)
            {
                var dc = pc.CreateDataChannel(Label);
                HandleDc(id, dc);
            }
            else
            {
                pc.OnDataChannel += (dc) => HandleDc(id, dc);
            }
        }

        private void HandleDc(string id, RTCDataChannel dc)
        {
            if (dc.Label != Label)
            {
                return;
            }

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"New DataChannel: id={id} label={dc.Label}");
            }

            if (!dcDict.ContainsKey(id))
            {
                dcDict.Add(id, new List<RTCDataChannel>());
            }
            dcDict[id].Add(dc);

            dc.OnMessage = message =>
            {
                FireOnMessageReceived(Encoding.UTF8.GetString(message));
            };
        }

        private void ClosePc(string id)
        {
            if (!dcDict.ContainsKey(id))
            {
                return;
            }
            dcDict[id].ForEach(dc => dc.Close());
            dcDict.Remove(id);
        }

        protected override void DoSend(string message)
        {
            foreach (var id in dcDict.Keys)
            {
                dcDict[id].ForEach(dc => dc.Send(message));
            }
        }

        public override void Clear()
        {
            dcDict.Keys.ToList().ForEach(ClosePc);
            dcDict.Clear();
        }
    }
}
