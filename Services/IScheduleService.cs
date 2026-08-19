using Cobalt.SalesAgentSchedule.Models;

namespace Cobalt.SalesAgentSchedule.Services;

public interface IScheduleService
{
    IReadOnlyList<SalesAgent> GetAgents();

    /// <summary>Entries for the week at the given offset from the current week (0 = current,
    /// -1 = previous, 1 = next, and so on).</summary>
    IReadOnlyList<ScheduleEntry> GetEntries(int weekOffset);
}
