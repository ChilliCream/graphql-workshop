using System;
using System.Collections.Generic;

namespace ConferencePlanner.GraphQL.Common
{
    public abstract class Payload
    {
        protected Payload(string? clientMutationId) 
            : this(null, clientMutationId)
        {
        }

        protected Payload(IReadOnlyList<UserError>? errors, string? clientMutationId)
        {
            Errors = errors ?? Array.Empty<UserError>();
            ClientMutationId = clientMutationId;
        }

        public IReadOnlyList<UserError> Errors { get; }

        public string? ClientMutationId { get; }
    }
}