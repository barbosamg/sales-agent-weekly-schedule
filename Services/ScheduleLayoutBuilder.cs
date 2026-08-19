using Cobalt.SalesAgentSchedule.Models;

namespace Cobalt.SalesAgentSchedule.Services;

public record ScheduleLayout(
    IReadOnlyDictionary<(int AgentId, DayOfWeek Day), IReadOnlyList<PositionedBlock>> BlocksByAgentAndDay,
    IReadOnlyDictionary<int, int> LaneCountByAgent);

/// <summary>
/// Assigns each entry a rendering lane (greedy interval scheduling, like meeting-room
/// allocation) so overlapping shifts for the same agent/day stack instead of collide,
/// and flags entries that overlap another as conflicts.
/// </summary>
public static class ScheduleLayoutBuilder
{
    public static ScheduleLayout Build(IEnumerable<ScheduleEntry> entries)
    {
        var blocksByGroup = new Dictionary<(int, DayOfWeek), IReadOnlyList<PositionedBlock>>();
        var laneCountByAgent = new Dictionary<int, int>();

        foreach (var group in entries.GroupBy(e => (e.AgentId, e.Day)))
        {
            var sorted = group.OrderBy(e => e.Start).ThenBy(e => e.End).ToList();
            var laneEnds = new List<TimeOnly>();
            var positioned = new List<PositionedBlock>();

            foreach (var entry in sorted)
            {
                int lane = laneEnds.FindIndex(end => end <= entry.Start);
                if (lane == -1)
                {
                    lane = laneEnds.Count;
                    laneEnds.Add(entry.End);
                }
                else
                {
                    laneEnds[lane] = entry.End;
                }

                bool isConflict = sorted.Any(other => other.Id != entry.Id && Overlaps(entry, other));
                positioned.Add(new PositionedBlock(entry, lane, isConflict));
            }

            blocksByGroup[group.Key] = positioned;

            int agentId = group.Key.AgentId;
            laneCountByAgent[agentId] = Math.Max(laneCountByAgent.GetValueOrDefault(agentId, 1), laneEnds.Count);
        }

        return new ScheduleLayout(blocksByGroup, laneCountByAgent);
    }

    private static bool Overlaps(ScheduleEntry a, ScheduleEntry b) => a.Start < b.End && b.Start < a.End;
}
