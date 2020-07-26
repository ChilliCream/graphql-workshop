using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers
{
    public class SpeakerPayloadBase : PayloadBase
    {
        public SpeakerPayloadBase(Speaker speaker, string? clientMutationId)
            : base(clientMutationId)
        {
            Speaker = speaker;
        }

        public SpeakerPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Speaker? Speaker { get; }
    }
}