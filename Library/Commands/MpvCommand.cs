using System.Text.Json.Serialization;

namespace Rustle.Library.Commands;

internal abstract record MpvCommand(
    [property: JsonPropertyName("request_id")]
    int RequestId
)
{
    [JsonPropertyName("command")]
    public abstract object[] Command { get; set; }
}