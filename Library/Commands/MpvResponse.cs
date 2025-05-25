using System.Text.Json.Serialization;

namespace Rustle.Library.Commands;

internal record MpvResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }

    [JsonPropertyName("request_id")]
    public required int RequestId { get; set; }

    public bool IsSuccess => Error == "success";
}