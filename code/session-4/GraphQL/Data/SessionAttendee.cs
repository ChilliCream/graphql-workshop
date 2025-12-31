namespace ConferencePlanner.GraphQL.Data;

public sealed class SessionAttendee
{
    public int SessionId { get; init; }

    public Session Session { get; init; } = null!;

    public int AttendeeId { get; init; }

    public Attendee Attendee { get; init; } = null!;
}
