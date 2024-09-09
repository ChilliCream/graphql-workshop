using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions;

[SubscriptionType]
public static class SessionSubscriptions
{
    [Subscribe]
    [Topic]
    public static async Task<Session> OnSessionScheduledAsync(
        [EventMessage] int sessionId,
        ISessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadRequiredAsync(sessionId, cancellationToken);
    }
}
