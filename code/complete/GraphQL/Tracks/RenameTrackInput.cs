using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Tracks
{
    public record RenameTrackInput(
        [property: ID(nameof(Track))] int Id, 
        string Name);
}