using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class AttendeePayloadBase : PayloadBase
    {
        public AttendeePayloadBase(Attendee attendee, string? clientMutationId)
            : base(clientMutationId)
        {
            Attendee = attendee;
        }

        public AttendeePayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Attendee? Attendee { get; }
    }
}