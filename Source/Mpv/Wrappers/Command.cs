using System.Text.Json.Serialization;

namespace Rustle.Mpv.Wrappers;

internal abstract record Command(
    [property: JsonPropertyName("request_id")]
    int RequestId
)
{
    [JsonPropertyName("command")]
    public abstract object[] Do { get; set; }
}

internal record StopCommand(int RequestId) : Command(RequestId)
{
    public override object[] Do { get; set; } = ["stop"];
}

internal record LoadfileCommand(int RequestId, string Url) : Command(RequestId)
{
    public override object[] Do { get; set; } = ["loadfile", Url];
}

internal record SetPropertyCommand(int RequestId, string Property, object Value) : Command(RequestId)
{
    public override object[] Do { get; set; } = ["set_property", Property, Value];
}

internal record GetPropertyCommand(int RequestId, string Property) : Command(RequestId)
{
    public override object[] Do { get; set; } = ["get_property", Property];
}

internal record ObservePropertyCommand(int RequestId, string Property, int Id = 0) : Command(RequestId)
{
    public override object[] Do { get; set; } = ["observe_property", Id, Property];
}