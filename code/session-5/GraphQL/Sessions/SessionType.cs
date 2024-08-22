using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

[ObjectType<Session>]
public static partial class SessionType
{
    static partial void Configure(IObjectTypeDescriptor<Session> descriptor)
    {
        descriptor
            .Ignore(s => s.SessionSpeakers)
            .Ignore(s => s.SessionAttendees);

        descriptor
            .Field(s => s.TrackId)
            .ID();
    }

    public static async Task<IEnumerable<Speaker>> GetSpeakersAsync(
        [Parent] Session session,
        ApplicationDbContext dbContext,
        SpeakerByIdDataLoader speakerById,
        CancellationToken cancellationToken)
    {
        var speakerIds = await dbContext.Sessions
            .Where(s => s.Id == session.Id)
            .Include(s => s.SessionSpeakers)
            .SelectMany(s => s.SessionSpeakers.Select(ss => ss.SpeakerId))
            .ToArrayAsync(cancellationToken);

        return await speakerById.LoadRequiredAsync(speakerIds, cancellationToken);
    }

    public static async Task<IEnumerable<Attendee>> GetAttendeesAsync(
        [Parent] Session session,
        ApplicationDbContext dbContext,
        AttendeeByIdDataLoader attendeeById,
        CancellationToken cancellationToken)
    {
        var attendeeIds = await dbContext.Sessions
            .Where(s => s.Id == session.Id)
            .Include(s => s.SessionAttendees)
            .SelectMany(s => s.SessionAttendees.Select(sa => sa.AttendeeId))
            .ToArrayAsync(cancellationToken);

        return await attendeeById.LoadRequiredAsync(attendeeIds, cancellationToken);
    }

    public static async Task<Track?> GetTrackAsync(
        [Parent] Session session,
        TrackByIdDataLoader trackById,
        CancellationToken cancellationToken)
    {
        if (session.TrackId is null)
        {
            return null;
        }

        return await trackById.LoadAsync(session.TrackId.Value, cancellationToken);
    }
}
