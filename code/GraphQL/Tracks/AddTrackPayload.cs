using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class AddTrackPayload : TrackPayloadBase
    {
        public AddTrackPayload(Track track, string? clientMutationId) 
            : base(track, clientMutationId)
        {
        }

        public AddTrackPayload(IReadOnlyList<UserError> errors, string? clientMutationId) 
            : base(errors, clientMutationId)
        {
        }
    }
}