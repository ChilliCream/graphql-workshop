using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Subscription")]
    public class AttendeeSubscriptions
    {
        public async ValueTask<IAsyncEnumerable<int>> SubscribeToOnAttendeeCheckedInAsync(
            int sessionId,
            [Service] ITopicEventReceiver eventReceiver,
            CancellationToken cancellationToken) =>
            await eventReceiver.SubscribeAsync<string, int>(
                "OnAttendeeCheckedIn_" + sessionId, cancellationToken);

        [Subscribe(With = nameof(SubscribeToOnAttendeeCheckedInAsync))]
        public SessionAttendeeCheckIn OnAttendeeCheckedInAsync(
            [ID(nameof(Session))] int sessionId,
            [EventMessage] int attendeeId,
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken) =>
            new SessionAttendeeCheckIn(attendeeId, sessionId);
    }
}