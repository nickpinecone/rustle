using System.Text.Json.Serialization;

namespace Rustle.Library.Commands;

internal record LoadfileCommand(
    int RequestId,
    [property: JsonIgnore] string Url
) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["loadfile", Url];
}

internal record StopCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["stop"];
}

internal record PauseCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["set_property", "pause", true];
}

internal record ResumeCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["set_property", "pause", false];
}

internal record GetPauseCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["get_property", "pause"];
}

internal record GetPauseResponse : MpvResponse
{
    [JsonPropertyName("data")]
    public required bool IsPaused { get; set; }
}

internal record SetVolumeCommand(
    int RequestId,
    [property: JsonIgnore] int Volume
) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["set_property", "volume", Volume];
}

internal record GetVolumeCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["get_property", "volume"];
}

internal record GetVolumeResponse : MpvResponse
{
    [JsonPropertyName("data")]
    public required float Volume { get; set; }
}

internal record GetMediaTitleCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["get_property", "media-title"];
}

internal record GetMediaTitleResponse : MpvResponse
{
    [JsonPropertyName("data")]
    public required string MediaTitle { get; set; }
}