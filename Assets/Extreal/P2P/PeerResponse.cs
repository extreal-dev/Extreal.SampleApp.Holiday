using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Extreal.P2P.Dev
{
    [SuppressMessage("Usage", "CC0047")]
    public class StartHostResponse
    {
        [JsonPropertyName("status")]
        public ushort Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class ListHostsResponse
    {
        [JsonPropertyName("status")]
        public ushort Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("hosts")]
        public List<HostResponse> Hosts { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class HostResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
