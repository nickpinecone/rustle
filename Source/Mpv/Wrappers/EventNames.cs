using System;

namespace Rustle.Mpv.Wrappers;

internal static class EventNames
{
    public const string PropertyChange = "property-change";
    public const string EndFile = "end-file";

    public static Type GetEventType(this Event @event)
    {
        return @event.EventName switch
        {
            PropertyChange => typeof(PropertyChangeEvent),
            EndFile => typeof(EndFileEvent),
            _ => typeof(Event)
        };
    }
}