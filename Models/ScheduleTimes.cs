namespace Cobalt.SalesAgentSchedule.Models;

/// <summary>Shared constants for the 7 AM-7 PM / twelve-slot working window.</summary>
public static class ScheduleTimes
{
    public const int StartHour = 7;
    public const int EndHour = 19;
    public const int TotalHours = EndHour - StartHour;
}
