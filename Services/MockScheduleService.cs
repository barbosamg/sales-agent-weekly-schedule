using Cobalt.SalesAgentSchedule.Models;

namespace Cobalt.SalesAgentSchedule.Services;

/// <summary>
/// Serves local, in-memory sample data (no API/DB, as required by the challenge).
/// There are three hand-authored weekly patterns (A/B/C); GetEntries picks one
/// deterministically from the navigated week offset so Previous/Current/Next Week
/// visibly show different, reproducible data instead of an identical repeat — while
/// still not requiring a real calendar backend.
/// </summary>
public class MockScheduleService : IScheduleService
{
    private static readonly IReadOnlyList<SalesAgent> Agents = new List<SalesAgent>
    {
        new(1, "Ava Bennett", "Senior Sales Agent"),
        new(2, "Marcus Cole", "Sales Agent"),
        new(3, "Priya Nair", "Sales Agent"),
        new(4, "Diego Alvarez", "Senior Sales Agent"),
        new(5, "Sarah Kim", "Sales Agent"),
        new(6, "Jordan Lee", "Sales Agent"),
    };

    // Pattern A — current week
    private static readonly IReadOnlyList<ScheduleEntry> PatternA = new List<ScheduleEntry>
    {
        new(1, 1, DayOfWeek.Monday, new(8, 0), new(12, 0), "Front Desk Coverage", "Customer A", ShiftKind.Confirmed),
        new(2, 1, DayOfWeek.Monday, new(13, 0), new(17, 0), "Sales Follow-up", "Customer B", ShiftKind.Confirmed),
        new(3, 1, DayOfWeek.Wednesday, new(7, 0), new(15, 0), "Full Shift", null, ShiftKind.Confirmed),
        new(4, 1, DayOfWeek.Friday, new(9, 0), new(11, 0), "Client Meeting", "Customer C", ShiftKind.Tentative),

        new(5, 2, DayOfWeek.Monday, new(7, 0), new(11, 0), "Cold Calling", null, ShiftKind.Confirmed),
        new(6, 2, DayOfWeek.Monday, new(10, 0), new(13, 0), "Follow-up Calls", null, ShiftKind.Confirmed),
        new(7, 2, DayOfWeek.Tuesday, new(7, 0), new(19, 0), "Extended Shift", null, ShiftKind.Confirmed),
        new(8, 2, DayOfWeek.Thursday, new(14, 0), new(16, 0), "Product Demo", "Customer D", ShiftKind.Confirmed),

        new(9, 3, DayOfWeek.Tuesday, new(8, 0), new(10, 0), "Client Onboarding", "Customer E", ShiftKind.Confirmed),
        new(10, 3, DayOfWeek.Tuesday, new(9, 0), new(12, 0), "Internal Training", null, ShiftKind.Tentative),
        new(11, 3, DayOfWeek.Thursday, new(7, 0), new(15, 0), "Full Shift", null, ShiftKind.Confirmed),
        new(12, 3, DayOfWeek.Saturday, new(7, 0), new(11, 0), "Saturday Coverage", "Customer F", ShiftKind.Confirmed),

        new(13, 4, DayOfWeek.Wednesday, new(10, 0), new(14, 0), "Contract Negotiation", "Customer G", ShiftKind.Confirmed),
        new(14, 4, DayOfWeek.Wednesday, new(13, 0), new(15, 0), "Internal Sync", null, ShiftKind.Tentative),
        new(15, 4, DayOfWeek.Friday, new(7, 0), new(12, 0), "Morning Shift", null, ShiftKind.Confirmed),
        // Fully packed day: 4 distinct, back-to-back entries covering the entire 7 AM-7 PM
        // window with zero gaps and zero overlap (each ends exactly when the next starts).
        new(22, 4, DayOfWeek.Thursday, new(7, 0), new(10, 0), "Morning Calls", null, ShiftKind.Confirmed),
        new(23, 4, DayOfWeek.Thursday, new(10, 0), new(13, 0), "Client Meetings", "Customer K", ShiftKind.Confirmed),
        new(24, 4, DayOfWeek.Thursday, new(13, 0), new(16, 0), "Afternoon Follow-ups", null, ShiftKind.Tentative),
        new(25, 4, DayOfWeek.Thursday, new(16, 0), new(19, 0), "Wrap-up & Reports", null, ShiftKind.Confirmed),

        new(16, 5, DayOfWeek.Monday, new(7, 0), new(19, 0), "Extended Shift", null, ShiftKind.Confirmed),
        new(17, 5, DayOfWeek.Thursday, new(10, 0), new(12, 0), "Site Visit", "Customer H", ShiftKind.Confirmed),
        new(18, 5, DayOfWeek.Saturday, new(8, 0), new(10, 0), "Quick Consult", "Customer I", ShiftKind.Confirmed),

        new(19, 6, DayOfWeek.Tuesday, new(7, 0), new(9, 0), "Store Opening", null, ShiftKind.Confirmed),
        new(20, 6, DayOfWeek.Wednesday, new(15, 0), new(19, 0), "Store Closing", null, ShiftKind.Confirmed),
        new(21, 6, DayOfWeek.Friday, new(13, 0), new(17, 0), "After-sales Support", "Customer J", ShiftKind.Confirmed),
        // Starts before 7 AM and ends after 7 PM - visually clamped to the full 7-19 window
        // by ScheduleBlock's VisibleStartHours/VisibleEndHours, real time still shown in the
        // tooltip/dialog.
        new(26, 6, DayOfWeek.Saturday, new(4, 0), new(22, 0), "All-Day Coverage", null, ShiftKind.Confirmed),
    };

    // Pattern B — next week
    private static readonly IReadOnlyList<ScheduleEntry> PatternB = new List<ScheduleEntry>
    {
        new(101, 1, DayOfWeek.Tuesday, new(7, 0), new(15, 0), "Full Shift", null, ShiftKind.Confirmed),
        new(102, 1, DayOfWeek.Thursday, new(8, 0), new(12, 0), "Front Desk Coverage", "Customer K", ShiftKind.Confirmed),
        new(103, 1, DayOfWeek.Friday, new(13, 0), new(15, 0), "Client Meeting", "Customer L", ShiftKind.Tentative),

        new(104, 2, DayOfWeek.Monday, new(9, 0), new(13, 0), "Sales Calls", null, ShiftKind.Confirmed),
        new(105, 2, DayOfWeek.Wednesday, new(7, 0), new(19, 0), "Extended Shift", null, ShiftKind.Confirmed),
        new(106, 2, DayOfWeek.Friday, new(7, 0), new(10, 0), "Morning Prep", null, ShiftKind.Confirmed),

        new(107, 3, DayOfWeek.Monday, new(7, 0), new(9, 0), "Team Standup", null, ShiftKind.Confirmed),
        new(108, 3, DayOfWeek.Monday, new(8, 0), new(11, 0), "Client Calls", null, ShiftKind.Confirmed),
        new(109, 3, DayOfWeek.Wednesday, new(10, 0), new(14, 0), "Client Onboarding", "Customer M", ShiftKind.Confirmed),
        new(110, 3, DayOfWeek.Saturday, new(9, 0), new(13, 0), "Saturday Coverage", "Customer N", ShiftKind.Confirmed),

        new(111, 4, DayOfWeek.Tuesday, new(8, 0), new(12, 0), "Contract Review", "Customer O", ShiftKind.Confirmed),
        new(112, 4, DayOfWeek.Tuesday, new(11, 0), new(13, 0), "Internal Sync", null, ShiftKind.Tentative),
        new(113, 4, DayOfWeek.Thursday, new(7, 0), new(15, 0), "Full Shift", null, ShiftKind.Confirmed),

        new(114, 5, DayOfWeek.Wednesday, new(8, 0), new(10, 0), "Site Visit", "Customer P", ShiftKind.Confirmed),
        new(115, 5, DayOfWeek.Friday, new(7, 0), new(19, 0), "Extended Shift", null, ShiftKind.Confirmed),

        new(116, 6, DayOfWeek.Monday, new(13, 0), new(17, 0), "After-sales Support", "Customer Q", ShiftKind.Confirmed),
        new(117, 6, DayOfWeek.Thursday, new(15, 0), new(19, 0), "Store Closing", null, ShiftKind.Confirmed),
        new(118, 6, DayOfWeek.Saturday, new(7, 0), new(9, 0), "Store Opening", null, ShiftKind.Confirmed),
    };

    // Pattern C — previous week (and cyclically, every third week beyond that)
    private static readonly IReadOnlyList<ScheduleEntry> PatternC = new List<ScheduleEntry>
    {
        new(201, 1, DayOfWeek.Monday, new(7, 0), new(11, 0), "Client Calls", "Customer R", ShiftKind.Confirmed),
        new(202, 1, DayOfWeek.Wednesday, new(13, 0), new(17, 0), "Sales Follow-up", "Customer S", ShiftKind.Confirmed),
        new(203, 1, DayOfWeek.Friday, new(7, 0), new(15, 0), "Full Shift", null, ShiftKind.Confirmed),

        new(204, 2, DayOfWeek.Tuesday, new(9, 0), new(13, 0), "Cold Calling", null, ShiftKind.Confirmed),
        new(205, 2, DayOfWeek.Tuesday, new(12, 0), new(14, 0), "Team Sync", null, ShiftKind.Tentative),
        new(206, 2, DayOfWeek.Thursday, new(7, 0), new(19, 0), "Extended Shift", null, ShiftKind.Confirmed),

        new(207, 3, DayOfWeek.Wednesday, new(7, 0), new(9, 0), "Store Opening", null, ShiftKind.Confirmed),
        new(208, 3, DayOfWeek.Friday, new(10, 0), new(12, 0), "Client Onboarding", "Customer T", ShiftKind.Confirmed),

        new(209, 4, DayOfWeek.Monday, new(8, 0), new(12, 0), "Contract Negotiation", "Customer U", ShiftKind.Confirmed),
        new(210, 4, DayOfWeek.Monday, new(11, 0), new(13, 0), "Internal Review", null, ShiftKind.Confirmed),
        new(211, 4, DayOfWeek.Saturday, new(7, 0), new(11, 0), "Saturday Coverage", "Customer V", ShiftKind.Confirmed),

        new(212, 5, DayOfWeek.Tuesday, new(7, 0), new(15, 0), "Full Shift", null, ShiftKind.Confirmed),
        new(213, 5, DayOfWeek.Thursday, new(13, 0), new(17, 0), "Site Visits", "Customer W", ShiftKind.Confirmed),

        new(214, 6, DayOfWeek.Wednesday, new(9, 0), new(13, 0), "After-sales Support", "Customer X", ShiftKind.Confirmed),
        new(215, 6, DayOfWeek.Friday, new(15, 0), new(19, 0), "Store Closing", null, ShiftKind.Tentative),
    };

    public IReadOnlyList<SalesAgent> GetAgents() => Agents;

    public IReadOnlyList<ScheduleEntry> GetEntries(int weekOffset)
    {
        var pattern = ((weekOffset % 3) + 3) % 3;
        return pattern switch
        {
            0 => PatternA,
            1 => PatternB,
            _ => PatternC,
        };
    }
}
