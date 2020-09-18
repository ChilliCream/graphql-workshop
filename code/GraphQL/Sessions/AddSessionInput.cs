using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public record AddSessionInput : Input
    {
        public AddSessionInput(
            string title,
            string? @abstract,
            IReadOnlyList<int> speakerIds,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Title = title;
            Abstract = @abstract;
            SpeakerIds = speakerIds;
        }

        public string Title { get; init; }

        public string? Abstract { get; init; }

        [ID(nameof(Speaker))]
        public IReadOnlyList<int> SpeakerIds { get; init; }
    }
}