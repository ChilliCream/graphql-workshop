using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Tracks;
using GreenDonut.Data;
using HotChocolate.Execution.Processing;

namespace ConferencePlanner.GraphQL.Sessions;

[ObjectType<Session>]
public static partial class SessionType
{
    static partial void Configure(IObjectTypeDescriptor<Session> descriptor)
    {
        descriptor
            .Field(s => s.TrackId)
            .ID<Track>();
    }

    public static TimeSpan Duration([Parent("StartTime EndTime")] Session session)
        => session.Duration;

    [BindMember(nameof(Session.SessionSpeakers))]
    public static async Task<IEnumerable<Speaker>> GetSpeakersAsync(
        [Parent] Session session,
        ISpeakersBySessionIdDataLoader speakersBySessionId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await speakersBySessionId
            .Select(selection)
            .LoadRequiredAsync(session.Id, cancellationToken);
    }

    [BindMember(nameof(Session.SessionAttendees))]
    public static async Task<IEnumerable<Attendee>> GetAttendeesAsync(
        [Parent(nameof(Session.Id))] Session session,
        IAttendeesBySessionIdDataLoader attendeesBySessionId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await attendeesBySessionId
            .Select(selection)
            .LoadRequiredAsync(session.Id, cancellationToken);
    }

    public static async Task<Track?> GetTrackAsync(
        [Parent(nameof(Session.TrackId))] Session session,
        ITrackByIdDataLoader trackById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        if (session.TrackId is null)
        {
            return null;
        }

        return await trackById
            .Select(selection)
            .LoadAsync(session.TrackId.Value, cancellationToken);
    }
}
