using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class SessionPayloadBase : PayloadBase
    {
        public SessionPayloadBase(Session session, string? clientMutationId)
            : base(clientMutationId)
        {
            Session = session;
        }

        public SessionPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Session? Session { get; }
    }
}