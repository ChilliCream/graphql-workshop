using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Types;

[ObjectType<Session>]
public static partial class SessionType
{
    public static TimeSpan Duration([Parent("StartTime EndTime")] Session session)
        => session.Duration;
}
