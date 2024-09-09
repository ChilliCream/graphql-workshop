using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Attendees;

public static class AttendeeDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Attendee>> AttendeeByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Attendees
            .Where(a => ids.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session[]>> SessionsByAttendeeIdAsync(
        IReadOnlyList<int> attendeeIds,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Attendees
            .Where(a => attendeeIds.Contains(a.Id))
            .Select(a => new { a.Id, Sessions = a.SessionsAttendees.Select(sa => sa.Session) })
            .ToDictionaryAsync(
                a => a.Id,
                a => a.Sessions.ToArray(),
                cancellationToken);
    }
}
