using ConferencePlanner.GraphQL.Data;
using GreenDonut.Selectors;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Speakers;

public static class SpeakerDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Speaker>> SpeakerByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Speakers
            .AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => s.Id, selector)
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session[]>> SessionsBySpeakerIdAsync(
        IReadOnlyList<int> speakerIds,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Speakers
            .AsNoTracking()
            .Where(s => speakerIds.Contains(s.Id))
            .Select(s => s.Id, s => s.SessionSpeakers.Select(ss => ss.Session), selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value.ToArray(), cancellationToken);
    }
}
