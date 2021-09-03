using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Types
{
    [Node]
    [ExtendObjectType(typeof(Speaker))]
    public class SpeakerNode
    {
        [BindMember(nameof(Speaker.SessionSpeakers), Replace = true)]
        public async Task<IEnumerable<Session>> GetSessionsAsync(
            [Parent] Speaker speaker,
            SessionBySpeakerIdDataLoader sessionBySpeakerId,
            CancellationToken cancellationToken)
            => await sessionBySpeakerId.LoadAsync(speaker.Id, cancellationToken);

        [NodeResolver]
        public static Task<Speaker> GetSpeakerByIdAsync(
            int id,
            SpeakerByIdDataLoader speakerById,
            CancellationToken cancellationToken)
            => speakerById.LoadAsync(id, cancellationToken);
    }
}