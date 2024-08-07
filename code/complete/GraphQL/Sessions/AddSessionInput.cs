using System.Collections.Generic;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public record AddSessionInput(
        string Title,
        string? Abstract,
        [property: ID(nameof(Speaker))]
        IReadOnlyList<int> SpeakerIds);
}