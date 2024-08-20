using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data;

public sealed class Attendee
{
    public int Id { get; set; }

    [StringLength(200)]
    public required string FirstName { get; set; }

    [StringLength(200)]
    public required string LastName { get; set; }

    [StringLength(200)]
    public required string Username { get; set; }

    [StringLength(256)]
    public string? EmailAddress { get; set; }

    public ICollection<SessionAttendee> SessionsAttendees { get; set; } =
        new List<SessionAttendee>();
}
