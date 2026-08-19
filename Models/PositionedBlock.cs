namespace Cobalt.SalesAgentSchedule.Models;

/// <summary>A ScheduleEntry placed into a rendering lane, with conflict state resolved.</summary>
public record PositionedBlock(ScheduleEntry Entry, int Lane, bool IsConflict);
