using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class AttendeePayloadBase : Payload
    {
        protected AttendeePayloadBase(Attendee attendee, string? clientMutationId)
            : base(clientMutationId)
        {
        }

        protected AttendeePayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Attendee? Attendee { get; }
    }
}