using System;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class ScheduleSessionInput : InputBase
    {
        public ScheduleSessionInput(
            int sessionId,
            int trackId,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string? clientMutationId)
            : base(clientMutationId)
        {
            SessionId = sessionId;
            TrackId = trackId;
            StartTime = startTime;
            EndTime = endTime;
        }

        [ID(nameof(Session))]
        public int SessionId { get; }

        [ID(nameof(Track))]
        public int TrackId { get; }

        public DateTimeOffset StartTime { get; }

        public DateTimeOffset EndTime { get; }
    }
}