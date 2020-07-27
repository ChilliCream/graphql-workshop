using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class RenameTrackInput : InputBase
    {
        public RenameTrackInput(
            int id,
            string name,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Id = id;
            Name = name;
        }

        [ID(nameof(Track))]
        public int Id { get; }

        public string Name { get; }
    }
}