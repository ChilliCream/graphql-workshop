using ConferencePlanner.GraphQL.Data;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks;

public static class TrackDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Track>> TrackByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tracks
            .AsNoTracking()
            .Where(t => ids.Contains(t.Id))
            .Select(t => t.Id, selector)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session[]>> SessionsByTrackIdAsync(
        IReadOnlyList<int> trackIds,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tracks
            .AsNoTracking()
            .Where(t => trackIds.Contains(t.Id))
            .Select(t => t.Id, t => t.Sessions, selector)
            .ToDictionaryAsync(r => r.Key, r => r.Value.ToArray(), cancellationToken);
    }
}
