using ConferencePlanner.GraphQL.Common;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class AddTrackInput : InputBase
    {
        public AddTrackInput(
            string name,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Name = name;
        }

        public string Name { get; }
    }
}