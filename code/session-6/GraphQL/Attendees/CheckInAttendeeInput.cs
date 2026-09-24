using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees;

public sealed record CheckInAttendeeInput(
    [property: ID<Session>] int SessionId,
    [property: ID<Attendee>] int AttendeeId);
