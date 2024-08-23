using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks;

[QueryType]
public static class TrackQueries
{
    [UsePaging]
    public static IQueryable<Track> GetTracks(ApplicationDbContext dbContext)
    {
        return dbContext.Tracks.OrderBy(t => t.Name);
    }

    [NodeResolver]
    public static async Task<Track?> GetTrackByIdAsync(
        int id,
        TrackByIdDataLoader trackById,
        CancellationToken cancellationToken)
    {
        return await trackById.LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Track>> GetTracksByIdAsync(
        [ID<Track>] int[] ids,
        TrackByIdDataLoader trackById,
        CancellationToken cancellationToken)
    {
        return await trackById.LoadRequiredAsync(ids, cancellationToken);
    }
}
