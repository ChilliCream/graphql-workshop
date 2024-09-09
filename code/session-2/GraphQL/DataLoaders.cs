using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL;

public static class DataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Speaker>> SpeakerByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Speakers
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session[]>> SessionsBySpeakerIdAsync(
        IReadOnlyList<int> speakerIds,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Speakers
            .AsNoTracking()
            .Where(s => speakerIds.Contains(s.Id))
            .Select(s => new { s.Id, Sessions = s.SessionSpeakers.Select(ss => ss.Session) })
            .ToDictionaryAsync(
                s => s.Id,
                s => s.Sessions.ToArray(),
                cancellationToken);
    }
}
