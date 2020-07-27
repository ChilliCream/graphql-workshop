using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class TrackPayloadBase : PayloadBase
    {
        public TrackPayloadBase(Track track, string? clientMutationId)
            : base(clientMutationId)
        {
            Track = track;
        }

        public TrackPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Track? Track { get; }
    }
}