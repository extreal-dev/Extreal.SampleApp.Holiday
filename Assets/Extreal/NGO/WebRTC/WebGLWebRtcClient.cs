﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using AOT;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Web.Common;
using Unity.Netcode;

namespace Extreal.NGO.WebRTC.Dev
{
    public class WebGLWebRtcClient : WebRtcClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(WebGLWebRtcClient));
        private static readonly WebGLWebRtcConfig Config = new WebGLWebRtcConfig
        {
            NgoServerClientId = NetworkManager.ServerClientId,
            IsDebug = Logger.IsDebug()
        };
        private static readonly string JsonConfig = JsonSerializer.Serialize(Config);

        private static WebGLWebRtcClient instance;

        public WebGLWebRtcClient()
        {
            instance = this;
            WebGLHelper.CallAction(WithPrefix(nameof(WebGLWebRtcClient)), JsonConfig);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnConnected)), HandleOnConnected);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnDataReceived)), HandleOnDataReceived);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnDisconnected)), HandleOnDisconnected);
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnConnected(string clientId, string unused)
            => instance.FireOnConnected(ToUlong(clientId));

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnDataReceived(string clientId, string payload)
            => instance.FireOnDataReceived(ToUlong(clientId), payload);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnDisconnected(string clientId, string unused)
            => instance.FireOnDisconnected(ToUlong(clientId));

#pragma warning disable CS1998
        protected override async UniTask DoConnectAsync()
            => WebGLHelper.CallAction(WithPrefix(nameof(DoConnectAsync)));
#pragma warning restore CS1998

        protected override void DoSend(ulong clientId, string payload)
            => WebGLHelper.CallAction(WithPrefix(nameof(DoSend)), clientId.ToString(), payload);

        protected override void DoClear() => WebGLHelper.CallAction(WithPrefix(nameof(DoClear)));

        public override void DisconnectRemoteClient(ulong clientId)
            => WebGLHelper.CallAction(WithPrefix(nameof(DisconnectRemoteClient)), clientId.ToString());

        private static string WithPrefix(string name) => $"{nameof(WebGLWebRtcClient)}#{name}";

        private static ulong ToUlong(string str) => Convert.ToUInt64(str);
    }

    [SuppressMessage("Usage", "CC0047")]
    public class WebGLWebRtcConfig
    {
        [JsonPropertyName("ngoServerClientId")]
        public ulong NgoServerClientId { get; set; }

        [JsonPropertyName("isDebug")]
        public bool IsDebug { get; set; }
    }
}
