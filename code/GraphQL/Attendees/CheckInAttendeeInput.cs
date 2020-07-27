using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class CheckInAttendeeInput : InputBase
    {
        public CheckInAttendeeInput(
            int sessionId,
            int attendeeId,
            string? clientMutationId)
            : base(clientMutationId)
        {
            SessionId = sessionId;
            AttendeeId = attendeeId;
        }

        [ID(nameof(Session))]
        public int SessionId { get; }

        [ID(nameof(Attendee))]
        public int AttendeeId { get; }
    }
}