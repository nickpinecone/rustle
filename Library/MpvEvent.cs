using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rustle.Library;

internal record MpvEvent
{
    [JsonPropertyName("event")]
    public required string Event { get; set; }
}

internal record PropertyChangeEvent : MpvEvent
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    
    [JsonPropertyName("data")]
    public required JsonElement Data { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

internal record EndFileEvent : MpvEvent
{
    [JsonPropertyName("reason")]
    public required string Reason { get; set; }
    
    [JsonPropertyName("playlist_insert_id")]
    public required int PlaylistInsertId { get; set; }
}