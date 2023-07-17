using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using AOT;
using Cysharp.Threading.Tasks;
using Extreal.WebGL;

namespace Extreal.P2P.Dev
{
    public class WebGLPeerClient : PeerClient
    {
        private static WebGLPeerClient instance;
        private StartHostResponse startHostResponse;
        private ListHostsResponse listHostsResponse;
        private CancellationTokenSource cancellation;

        public WebGLPeerClient(WebGLPeerConfig peerConfig)
        {
            instance = this;
            cancellation = new CancellationTokenSource();
            WebGLHelper.CallAction(WithPrefix(nameof(WebGLPeerClient)), ToJson(peerConfig));
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnStarted)), HandleOnStarted);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnConnectFailed)), HandleOnConnectFailed);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnDisconnected)), HandleOnDisconnected);
            WebGLHelper.AddCallback(WithPrefix(nameof(ReceiveStartHostResponse)), ReceiveStartHostResponse);
            WebGLHelper.AddCallback(WithPrefix(nameof(ReceiveListHostsResponse)), ReceiveListHostsResponse);
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnStarted(string unused1, string unused2) => instance.FireOnStarted();

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnConnectFailed(string reason, string unused2) =>
            instance.FireOnConnectFailed(reason);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnDisconnected(string reason, string unused2) => instance.FireOnDisconnected(reason);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ReceiveStartHostResponse(string jsonResponse, string unused)
            => instance.startHostResponse = JsonSerializer.Deserialize<StartHostResponse>(jsonResponse);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ReceiveListHostsResponse(string jsonResponse, string unused)
            => instance.listHostsResponse = JsonSerializer.Deserialize<ListHostsResponse>(jsonResponse);

        protected override void DoReleaseManagedResources() => cancellation?.Dispose();

        protected override async UniTask<StartHostResponse> DoStartHostAsync(string name)
        {
            WebGLHelper.CallAction(WithPrefix(nameof(DoStartHostAsync)), name);
            await UniTask.WaitUntil(() => startHostResponse != null, cancellationToken: cancellation.Token);
            var result = startHostResponse;
            startHostResponse = null;
            return result;
        }

        protected override async UniTask<ListHostsResponse> DoListHostsAsync()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(DoListHostsAsync)));
            await UniTask.WaitUntil(() => listHostsResponse != null, cancellationToken: cancellation.Token);
            var result = listHostsResponse;
            listHostsResponse = null;
            return result;
        }

        protected override UniTask DoStartClientAsync(string hostId)
#pragma warning disable CS1998
            => UniTask.Create(async () => WebGLHelper.CallAction(WithPrefix(nameof(DoStartClientAsync)), hostId));
#pragma warning restore CS1998

        protected override UniTask DoStopAsync()
        {
#pragma warning disable CS1998
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = new CancellationTokenSource();
            return UniTask.Create(async () => WebGLHelper.CallAction(WithPrefix(nameof(DoStopAsync))));
#pragma warning restore CS1998
        }

        private static string WithPrefix(string name) => $"{nameof(WebGLPeerClient)}#{name}";

        private static string ToJson(WebGLPeerConfig peerConfig)
        {
            var jsonRtcConfiguration = new JsonRtcConfiguration()
            {
                IceServers = peerConfig.IceServerUrls.Count > 0
                    ? new List<JsonRtcIceServer>
                    {
                        new JsonRtcIceServer { Urls = peerConfig.IceServerUrls.ToArray() },
                    }.ToArray()
                    : Array.Empty<JsonRtcIceServer>()
            };
            var socketOptions = peerConfig.SocketOptions;
            var jsonSocketOptions = new JsonSocketOptions
            {
                ConnectionTimeout = socketOptions.ConnectionTimeout.Milliseconds,
                Reconnection = socketOptions.Reconnection,
            };
            var jsonPeerConfig = new JsonPeerConfig
            {
                Url = peerConfig.SignalingUrl,
                SocketOptions = jsonSocketOptions,
                PcConfig = jsonRtcConfiguration,
                IsDebug = peerConfig.IsDebug
            };
            return JsonSerializer.Serialize(jsonPeerConfig);
        }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonPeerConfig
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("socketOptions")]
        public JsonSocketOptions SocketOptions { get; set; }

        [JsonPropertyName("pcConfig")]
        public JsonRtcConfiguration PcConfig { get; set; }

        [JsonPropertyName("isDebug")]
        public bool IsDebug { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonSocketOptions
    {
        [JsonPropertyName("connectionTimeout")]
        public int ConnectionTimeout { get; set; }

        [JsonPropertyName("reconnection")]
        public bool Reconnection { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonRtcConfiguration
    {
        [JsonPropertyName("iceServers")]
        public JsonRtcIceServer[] IceServers { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonRtcIceServer
    {
        [JsonPropertyName("urls")]
        public string[] Urls { get; set; }
    }
}
