using System;
using Extreal.Core.Common.System;

namespace Extreal.NGO.WebRTC.Dev
{
    public abstract class WebRtcClient : DisposableBase
    {
        protected WebRtcRole Role { get; set; }
        public abstract void Connect(WebRtcRole role);
        public abstract void Send(ulong clientId, ArraySegment<byte> payload);
        public abstract WebRtcEvent PollEvent();
        public abstract void Disconnect();
        public abstract void Shutdown();
    }
}
