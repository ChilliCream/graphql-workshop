using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public record RenameSessionInput(
        [property: ID(nameof(Session))] string SessionId,
        string Title);
}