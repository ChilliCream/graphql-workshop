using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class RegisterAttendeePayload : AttendeePayloadBase
    {
        public RegisterAttendeePayload(Attendee attendee, string? clientMutationId)
            : base(attendee, clientMutationId)
        {
        }

        public RegisterAttendeePayload(UserError error, string? clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }
    }
}