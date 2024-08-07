using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Tracks
{
    [Node]
    [ExtendObjectType(typeof(Track))]
    public class TrackNode
    {
        [UseUpperCase]
        public string GetName([Parent] Track track) => track.Name!;
        
        [UseApplicationDbContext]
        [UsePaging(ConnectionName = "TrackSessions")]
        public IQueryable<Session> GetSessions(
            [Parent] Track track,
            [ScopedService] ApplicationDbContext dbContext)
            => dbContext.Tracks.Where(t => t.Id == track.Id).SelectMany(t => t.Sessions);

        [NodeResolver]
        public static Task<Track> GetTrackByIdAsync(
            int id,
            TrackByIdDataLoader trackByIdDataLoader,
            CancellationToken cancellationToken)
            => trackByIdDataLoader.LoadAsync(id, cancellationToken); 
    }
}