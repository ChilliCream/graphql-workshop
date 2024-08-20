namespace ConferencePlanner.GraphQL.Data;

public sealed class SessionAttendee
{
    public int SessionId { get; init; }

    public Session? Session { get; init; }

    public int AttendeeId { get; init; }

    public Attendee? Attendee { get; init; }
}
