using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data;

public sealed class Session
{
    public int Id { get; init; }

    [StringLength(200)]
    public required string Title { get; init; }

    [StringLength(4000)]
    public string? Abstract { get; init; }

    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    // Bonus points to those who can figure out why this is written this way.
    public TimeSpan Duration =>
        EndTime?.Subtract(StartTime ?? EndTime ?? DateTimeOffset.MinValue) ??
        TimeSpan.Zero;

    public int? TrackId { get; set; }

    public ICollection<SessionSpeaker> SessionSpeakers { get; init; } =
        new List<SessionSpeaker>();

    public ICollection<SessionAttendee> SessionAttendees { get; init; } =
        new List<SessionAttendee>();

    public Track? Track { get; init; }
}
