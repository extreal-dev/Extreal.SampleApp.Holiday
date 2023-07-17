using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Extreal.WebGL
{
    public static class WebGLHelper
    {
        public static void Initialize(bool isDebug = false)
        {
            var webGLHelperConfig = new WebGLHelperConfig() { IsDebug = isDebug };
            Nop(JsonSerializer.Serialize(webGLHelperConfig));
        }

        [DllImport("__Internal")]
        private static extern void Nop(string str);

        [DllImport("__Internal")]
        public static extern void CallAction(string name, string str1 = "", string str2 = "");

        [DllImport("__Internal")]
        public static extern string CallFunction(string name, string str1 = "", string str2 = "");

        [DllImport("__Internal")]
        public static extern void AddCallback(string name, Action<string, string> action);
    }

    [SuppressMessage("Usage", "CC0047")]
    public class WebGLHelperConfig
    {
        [JsonPropertyName("isDebug")]
        public bool IsDebug { get; set; }
    }
}
