using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Extensions;

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
        ISessionsByTrackIdDataLoader sessionsByTrackId,
        CancellationToken cancellationToken)
    {
        return await sessionsByTrackId.LoadRequiredAsync(track.Id, cancellationToken);
    }
}
