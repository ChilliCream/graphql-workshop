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
    public class SessionByAttendeeIdDataLoader : GroupedDataLoader<int, Session>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SessionByAttendeeIdDataLoader(
            IBatchScheduler batchScheduler, 
            DbContextPool<ApplicationDbContext> dbContextPool)
            : base(batchScheduler)
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
                List<SessionAttendee> speakers = await dbContext.Attendees
                    .Where(speaker => keys.Contains(speaker.Id))
                    .Include(speaker => speaker.SessionsAttendees)
                    .SelectMany(speaker => speaker.SessionsAttendees)
                    .Include(sessionSpeaker => sessionSpeaker.Session)
                    .ToListAsync();

                return speakers
                    .Where(t => t.Session is { })
                    .ToLookup(t => t.AttendeeId, t => t.Session!);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}