using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Extensions;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

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

    [UsePaging]
    public static async Task<Connection<Session>> GetSessionsAsync(
        [Parent] Track track,
        ISessionsByTrackIdDataLoader sessionsByTrackId,
        PagingArguments pagingArguments,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await sessionsByTrackId
            .WithPagingArguments(pagingArguments)
            .Select(selection)
            .LoadAsync(track.Id, cancellationToken)
            .ToConnectionAsync();
    }
}
