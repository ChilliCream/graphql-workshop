using ConferencePlanner.GraphQL.Common;

namespace ConferencePlanner.GraphQL.Tracks
{
    public record AddTrackInput(
        string Name,
        string? ClientMutationId)
        : Input(ClientMutationId);
}