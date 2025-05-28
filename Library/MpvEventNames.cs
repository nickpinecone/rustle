using System;

namespace Rustle.Library;

internal static class MpvEventNames
{
    public const string PropertyChange = "property-change";
    public const string EndFile = "end-file";

    public static Type GetEventType(this MpvEvent @event)
    {
        return @event.Event switch
        {
            PropertyChange => typeof(PropertyChangeEvent),
            EndFile => typeof(EndFileEvent),
            _ => typeof(MpvEvent)
        };
    }
}