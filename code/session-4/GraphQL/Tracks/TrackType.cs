using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Extensions;
using GreenDonut.Projections;
using HotChocolate.Execution.Processing;

namespace ConferencePlanner.GraphQL.Tracks;

[ObjectType<Track>]
public static partial class TrackType
{
    static partial void Configure(IObjectTypeDescriptor<Track> descriptor)
    {
        descriptor
            .Field(t => t.Name)
            .ParentRequires(nameof(Track.Name))
            .UseUpperCase();
    }

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
