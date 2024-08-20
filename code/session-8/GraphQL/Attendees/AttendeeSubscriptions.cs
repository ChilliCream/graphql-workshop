using ConferencePlanner.GraphQL.Data;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Attendees;

[SubscriptionType]
public static class AttendeeSubscriptions
{
    [Subscribe(With = nameof(SubscribeToOnAttendeeCheckedInAsync))]
    public static SessionAttendeeCheckIn OnAttendeeCheckedIn(
        [ID<Session>] int sessionId,
        [EventMessage] int attendeeId)
    {
        return new SessionAttendeeCheckIn(attendeeId, sessionId);
    }

    public static async ValueTask<ISourceStream<int>> SubscribeToOnAttendeeCheckedInAsync(
        int sessionId,
        ITopicEventReceiver eventReceiver,
        CancellationToken cancellationToken)
    {
        return await eventReceiver.SubscribeAsync<int>(
            $"OnAttendeeCheckedIn_{sessionId}",
            cancellationToken);
    }
}
