using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Tracks
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class TrackMutations
    {
        [Input] 
        public async Task<Track> AddTrackAsync(
            string name,
            ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var track = new Track { Name = name };
            context.Tracks.Add(track);

            await context.SaveChangesAsync(cancellationToken);

            return track;
        }

        public async Task<RenameTrackPayload> RenameTrackAsync(
            RenameTrackInput input,
            ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var track = await context.Tracks.FindAsync(input.Id, cancellationToken);

            if (track is null)
            {
                throw new GraphQLException("Track not found.");
            }
            
            track.Name = input.Name;

            await context.SaveChangesAsync(cancellationToken);

            return new RenameTrackPayload(track);
        }
    }
}