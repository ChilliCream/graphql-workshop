using ConferencePlanner.GraphQL.Data;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

public static class SessionDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session>> SessionByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => s.Id, selector)
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Speaker[]>> SpeakersBySessionIdAsync(
        IReadOnlyList<int> sessionIds,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => s.Id, s => s.SessionSpeakers.Select(ss => ss.Speaker), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value.ToArray(), cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Attendee[]>> AttendeesBySessionIdAsync(
        IReadOnlyList<int> sessionIds,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => s.Id, s => s.SessionAttendees.Select(sa => sa.Attendee), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value.ToArray(), cancellationToken);
    }
}
