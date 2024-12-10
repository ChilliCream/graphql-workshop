using ConferencePlanner.GraphQL.Data;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Sessions;

[MutationType]
public static class SessionMutations
{
    [Error<TitleEmptyException>]
    [Error<NoSpeakerException>]
    public static async Task<Session> AddSessionAsync(
        AddSessionInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Title))
        {
            throw new TitleEmptyException();
        }

        if (input.SpeakerIds.Count == 0)
        {
            throw new NoSpeakerException();
        }

        var session = new Session
        {
            Title = input.Title,
            Abstract = input.Abstract
        };

        foreach (var speakerId in input.SpeakerIds)
        {
            session.SessionSpeakers.Add(new SessionSpeaker
            {
                SpeakerId = speakerId
            });
        }

        dbContext.Sessions.Add(session);

        await dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }

    [Error<EndTimeInvalidException>]
    [Error<SessionNotFoundException>]
    public static async Task<Session> ScheduleSessionAsync(
        ScheduleSessionInput input,
        ApplicationDbContext dbContext,
        ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        if (input.EndTime < input.StartTime)
        {
            throw new EndTimeInvalidException();
        }

        var session = await dbContext.Sessions.FindAsync([input.SessionId], cancellationToken);

        if (session is null)
        {
            throw new SessionNotFoundException();
        }

        session.TrackId = input.TrackId;
        session.StartTime = input.StartTime;
        session.EndTime = input.EndTime;

        await dbContext.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(
            nameof(SessionSubscriptions.OnSessionScheduledAsync),
            session.Id,
            cancellationToken);

        return session;
    }
}
