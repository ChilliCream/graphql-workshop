using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class AddSessionInput : InputBase
    {
        public AddSessionInput(
            string title,
            string? @abstract,
            List<int> speakerIds,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Title = title;
            Abstract = @abstract;
            SpeakerIds = speakerIds;
        }

        public string Title { get; }

        public string? Abstract { get; }

        [ID(nameof(Speaker))]
        public List<int> SpeakerIds { get; }
    }
}