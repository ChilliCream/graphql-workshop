using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Tracks
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class TrackQueries
    {
        [UsePaging]
        public IQueryable<Track> GetTracks(
            ApplicationDbContext context) 
            => context.Tracks.OrderBy(t => t.Name);

        public Task<Track> GetTrackByNameAsync(
            string name,
            ApplicationDbContext context,
            CancellationToken cancellationToken) 
            => context.Tracks.FirstAsync(t => t.Name == name, cancellationToken);

        public async Task<IEnumerable<Track>> GetTrackByNamesAsync(
            string[] names,
            ApplicationDbContext context,
            CancellationToken cancellationToken) 
            => await context.Tracks.Where(t => names.Contains(t.Name)).ToListAsync(cancellationToken);

        public Task<Track> GetTrackByIdAsync(
            [ID(nameof(Track))] int id,
            TrackByIdDataLoader trackById,
            CancellationToken cancellationToken) 
            => trackById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Track>> GetSessionsByIdAsync(
            [ID(nameof(Track))] int[] ids,
            TrackByIdDataLoader trackById,
            CancellationToken cancellationToken) 
            => await trackById.LoadAsync(ids, cancellationToken);
    }
}