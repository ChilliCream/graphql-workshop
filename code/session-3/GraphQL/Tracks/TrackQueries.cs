using ConferencePlanner.GraphQL.Data;
using GreenDonut.Projections;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks;

[QueryType]
public static class TrackQueries
{
    public static async Task<IEnumerable<Track>> GetTracksAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tracks.AsNoTracking().ToListAsync(cancellationToken);
    }

    [NodeResolver]
    public static async Task<Track?> GetTrackByIdAsync(
        int id,
        ITrackByIdDataLoader trackById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await trackById.Select(selection).LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Track>> GetTracksByIdAsync(
        [ID<Track>] int[] ids,
        ITrackByIdDataLoader trackById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await trackById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}
