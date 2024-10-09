using ConferencePlanner.GraphQL.Data;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks;

public static class TrackDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Track>> TrackByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tracks
            .AsNoTracking()
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Page<Session>>> SessionsByTrackIdAsync(
        IReadOnlyList<int> trackIds,
        ApplicationDbContext dbContext,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(s => s.TrackId != null && trackIds.Contains((int)s.TrackId))
            .OrderBy(s => s.Id)
            .ToBatchPageAsync(s => (int)s.TrackId!, pagingArguments, cancellationToken);
    }
}
