using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class SessionByTrackIdDataLoader : GroupedDataLoader<int, Session>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SessionByTrackIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Session>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                var sessions = await dbContext.Tracks
                    .Where(track => keys.Contains(track.Id))
                    .Include(track => track.Sessions)
                    .SelectMany(track => track.Sessions)
                    .ToListAsync();

                return sessions.ToLookup(t => t.TrackId!.Value);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}