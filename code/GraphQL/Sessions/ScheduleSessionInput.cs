using System;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public record ScheduleSessionInput(
        [ID(nameof(Session))]
        int SessionId,
        [ID(nameof(Track))]
        int TrackId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        string? ClientMutationId)
        : Input(ClientMutationId);
}