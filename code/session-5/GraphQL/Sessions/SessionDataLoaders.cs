using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

public static class SessionDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session>> SessionByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .Where(s => ids.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Speaker[]>> SpeakersBySessionIdAsync(
        IReadOnlyList<int> sessionIds,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => new { s.Id, Speakers = s.SessionSpeakers.Select(ss => ss.Speaker) })
            .ToDictionaryAsync(
                s => s.Id,
                s => s.Speakers.ToArray(),
                cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Attendee[]>> AttendeesBySessionIdAsync(
        IReadOnlyList<int> sessionIds,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => new { s.Id, Attendees = s.SessionAttendees.Select(sa => sa.Attendee) })
            .ToDictionaryAsync(
                s => s.Id,
                s => s.Attendees.ToArray(),
                cancellationToken);
    }
}
