using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rustle.Mpv.Wrappers;

internal record Event
{
    [JsonPropertyName("event")]
    public required string EventName { get; set; }
}

internal record PropertyChangeEvent : Event
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    
    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

internal static class EndFileReasons
{
    public const string Error = "error";
    public const string Eof = "eof";
}

internal record EndFileEvent : Event
{
    [JsonPropertyName("reason")]
    public required string Reason { get; set; }

    [JsonPropertyName("file_error")]
    public string FileError { get; set; } = "";
}