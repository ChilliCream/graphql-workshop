using System;
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
            IReadOnlyList<int> speakerIds,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Title = title;
            Abstract = @abstract;
            SpeakerIds = speakerIds;
        }

        public string Title { get; set; }

        public string? Abstract { get; set; }

        [ID(nameof(Speaker))]
        public IReadOnlyList<int> SpeakerIds { get; }
    }
}