
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
    public class SpeakerBySessionIdDataLoader : GroupedDataLoader<int, Speaker>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SpeakerBySessionIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Speaker>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                List<SessionSpeaker> speakers = await dbContext.Sessions
                    .Where(session => keys.Contains(session.Id))
                    .Include(session => session.SessionSpeakers)
                    .SelectMany(session => session.SessionSpeakers)
                    .Include(sessionSpeaker => sessionSpeaker.Speaker)
                    .ToListAsync();

                return speakers
                    .Where(t => t.Speaker is { })
                    .ToLookup(t => t.SessionId, t => t.Speaker!);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}