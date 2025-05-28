using System.Text.Json.Serialization;

namespace Rustle.Library;

internal abstract record MpvCommand(
    [property: JsonPropertyName("request_id")]
    int RequestId
)
{
    [JsonPropertyName("command")]
    public abstract object[] Command { get; set; }
}

internal record StopCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["stop"];
}

internal record LoadfileCommand(int RequestId, string Url) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["loadfile", Url];
}

internal record SetPropertyCommand(int RequestId, string Property, object Value) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["set_property", Property, Value];
}

internal record GetPropertyCommand(int RequestId, string Property) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["get_property", Property];
}

internal record ObservePropertyCommand(int RequestId, string Property, int Id = 0) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["observe_property", Id, Property];
}
