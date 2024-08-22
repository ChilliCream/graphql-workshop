using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Extensions;
using ConferencePlanner.GraphQL.Sessions;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Tracks;

[ObjectType<Track>]
public static partial class TrackType
{
    static partial void Configure(IObjectTypeDescriptor<Track> descriptor)
    {
        descriptor
            .Field(t => t.Name)
            .UseUpperCase();
    }

    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Track track,
        ApplicationDbContext dbContext,
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        var sessionIds = await dbContext.Sessions
            .Where(s => s.TrackId == track.Id)
            .Select(s => s.Id)
            .ToArrayAsync(cancellationToken);

        return await sessionById.LoadRequiredAsync(sessionIds, cancellationToken);
    }
}
