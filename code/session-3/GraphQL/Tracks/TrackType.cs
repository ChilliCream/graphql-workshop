using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks;

[ObjectType<Track>]
public static partial class TrackType
{
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Track track,
        SessionsByTrackIdDataLoader sessionsByTrackId,
        CancellationToken cancellationToken)
    {
        return await sessionsByTrackId.LoadRequiredAsync(track.Id, cancellationToken);
    }
}
