using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rustle.Mpv.Wrappers;

internal record Response
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }

    [JsonPropertyName("request_id")]
    public required int RequestId { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }

    public bool IsSuccess => Error == "success";
}