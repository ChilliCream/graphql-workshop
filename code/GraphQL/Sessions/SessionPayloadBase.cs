using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class SessionPayloadBase : Payload
    {
        protected SessionPayloadBase(Session session, string? clientMutationId)
            : base(clientMutationId)
        {
            Session = session;
        }

        protected SessionPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Session? Session { get; }
    }
}