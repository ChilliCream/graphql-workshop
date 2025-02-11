using ConferencePlanner.GraphQL.Data;
using GreenDonut.Data;
using HotChocolate.Execution.Processing;

namespace ConferencePlanner.GraphQL.Tracks;

[ObjectType<Track>]
public static partial class TrackType
{
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Track track,
        ISessionsByTrackIdDataLoader sessionsByTrackId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await sessionsByTrackId
            .Select(selection)
            .LoadRequiredAsync(track.Id, cancellationToken);
    }
}
