using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class RenameTrackPayload : TrackPayloadBase
    {
        public RenameTrackPayload(Track track, string? clientMutationId) 
            : base(track, clientMutationId)
        {
        }

        public RenameTrackPayload(IReadOnlyList<UserError> errors, string? clientMutationId) 
            : base(errors, clientMutationId)
        {
        }
    }
}