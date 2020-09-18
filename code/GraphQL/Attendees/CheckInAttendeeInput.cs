using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    public record CheckInAttendeeInput(
        [ID(nameof(Session))]
        int SessionId,
        [ID(nameof(Attendee))]
        int AttendeeId,
        string? ClientMutationId) 
        : Input(ClientMutationId);
}