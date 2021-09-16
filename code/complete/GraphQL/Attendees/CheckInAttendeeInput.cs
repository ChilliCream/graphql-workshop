using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    public record CheckInAttendeeInput(
        [property: ID(nameof(Session))]
        int SessionId,
        [property: ID(nameof(Attendee))]
        int AttendeeId);
}