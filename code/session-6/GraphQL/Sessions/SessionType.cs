using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Tracks;

namespace ConferencePlanner.GraphQL.Sessions;

[ObjectType<Session>]
public static partial class SessionType
{
    static partial void Configure(IObjectTypeDescriptor<Session> descriptor)
    {
        descriptor
            .Field(s => s.TrackId)
            .ID(nameof(Track));
    }

    [BindMember(nameof(Session.SessionSpeakers))]
    public static async Task<IEnumerable<Speaker>> GetSpeakersAsync(
        [Parent] Session session,
        ISpeakersBySessionIdDataLoader speakersBySessionId,
        CancellationToken cancellationToken)
    {
        return await speakersBySessionId.LoadRequiredAsync(session.Id, cancellationToken);
    }

    [BindMember(nameof(Session.SessionAttendees))]
    public static async Task<IEnumerable<Attendee>> GetAttendeesAsync(
        [Parent] Session session,
        IAttendeesBySessionIdDataLoader attendeesBySessionId,
        CancellationToken cancellationToken)
    {
        return await attendeesBySessionId.LoadRequiredAsync(session.Id, cancellationToken);
    }

    public static async Task<Track?> GetTrackAsync(
        [Parent] Session session,
        ITrackByIdDataLoader trackById,
        CancellationToken cancellationToken)
    {
        if (session.TrackId is null)
        {
            return null;
        }

        return await trackById.LoadAsync(session.TrackId.Value, cancellationToken);
    }
}
