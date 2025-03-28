using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions;

public sealed record ScheduleSessionInput(
    [property: ID<Session>] int SessionId,
    [property: ID<Track>] int TrackId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
