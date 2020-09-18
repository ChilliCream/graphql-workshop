using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class AddSessionPayload : Payload
    {
        public AddSessionPayload(Session session, string? clientMutationId)
            : base(clientMutationId)
        {
            Session = session;
        }

        public AddSessionPayload(UserError error, string? clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }

        public AddSessionPayload(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Session? Session { get; init; }
    }
}