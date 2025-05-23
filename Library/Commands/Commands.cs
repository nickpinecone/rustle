namespace Rayer.Library.Commands;

internal record PauseCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["set_property", "pause", true];
}

internal record ResumeCommand(int RequestId) : MpvCommand(RequestId)
{
    public override object[] Command { get; set; } = ["set_property", "pause", false];
}