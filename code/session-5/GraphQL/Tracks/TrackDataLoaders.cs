using ConferencePlanner.GraphQL.Data;
using GreenDonut.Selectors;
using HotChocolate.Pagination;
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
    public static async Task<IReadOnlyDictionary<int, Page<Session>>> SessionsByTrackIdAsync(
        IReadOnlyList<int> trackIds,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(s => s.TrackId != null && trackIds.Contains((int)s.TrackId))
            .OrderBy(s => s.Id)
            .Select(s => s.TrackId, selector)
            .ToBatchPageAsync(s => (int)s.TrackId!, pagingArguments, cancellationToken);
    }
}
