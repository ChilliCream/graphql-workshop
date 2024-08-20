using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions;

[SubscriptionType]
public static class SessionSubscriptions
{
    [Subscribe]
    [Topic]
    public static async Task<Session> OnSessionScheduledAsync(
        [EventMessage] int sessionId,
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadAsync(sessionId, cancellationToken);
    }
}
