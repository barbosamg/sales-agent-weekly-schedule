namespace Cobalt.SalesAgentSchedule.Models;

/// <summary>
/// A mock shift/appointment block. Day is a day-of-week template slot within a
/// specific weekly pattern (see MockScheduleService) rather than an absolute date.
/// </summary>
public record ScheduleEntry(
    int Id,
    int AgentId,
    DayOfWeek Day,
    TimeOnly Start,
    TimeOnly End,
    string Title,
    string? Customer,
    ShiftKind Kind);
