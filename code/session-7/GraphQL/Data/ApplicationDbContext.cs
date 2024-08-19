using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Attendee> Attendees { get; set; }

    public DbSet<Session> Sessions { get; set; }

    public DbSet<Speaker> Speakers { get; set; }

    public DbSet<Track> Tracks { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Attendee>()
            .HasIndex(a => a.Username)
            .IsUnique();

        // Many-to-many: Session <-> Attendee
        modelBuilder
            .Entity<SessionAttendee>()
            .HasKey(sa => new { sa.SessionId, sa.AttendeeId });

        // Many-to-many: Speaker <-> Session
        modelBuilder
            .Entity<SessionSpeaker>()
            .HasKey(ss => new { ss.SessionId, ss.SpeakerId });
    }
}
