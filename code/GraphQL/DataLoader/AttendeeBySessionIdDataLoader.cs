using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;
using GreenDonut;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class AttendeeBySessionIdDataLoader : GroupedDataLoader<int, Attendee>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public AttendeeBySessionIdDataLoader(
            IBatchScheduler batchScheduler, 
            DbContextPool<ApplicationDbContext> dbContextPool)
            : base(batchScheduler)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Attendee>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                List<SessionAttendee> speakers = await dbContext.Sessions
                    .Where(session => keys.Contains(session.Id))
                    .Include(session => session.SessionAttendees)
                    .SelectMany(session => session.SessionAttendees)
                    .ToListAsync();

                return speakers.Where(t => t.Attendee is { }).ToLookup(t => t.SessionId, t => t.Attendee!);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}