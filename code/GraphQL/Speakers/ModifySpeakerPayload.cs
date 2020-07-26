using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers
{
    public class ModifySpeakerPayload : SpeakerPayloadBase
    {
        public ModifySpeakerPayload(Speaker speaker, string? clientMutationId)
            : base(speaker, clientMutationId)
        {
        }

        public ModifySpeakerPayload(UserError error, string? clientMutationId)
            : base(new [] { error }, clientMutationId)
        {
        }

        public ModifySpeakerPayload(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }
    }
}