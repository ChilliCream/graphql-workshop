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
    public class SessionByIdDataLoader : BatchDataLoader<int, Session>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SessionByIdDataLoader(
            IBatchScheduler batchScheduler, 
            DbContextPool<ApplicationDbContext> dbContextPool)
            : base(batchScheduler)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<IReadOnlyDictionary<int, Session>> LoadBatchAsync(
            IReadOnlyList<int> keys, 
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                return await dbContext.Sessions
                    .Where(s => keys.Contains(s.Id))
                    .ToDictionaryAsync(t => t.Id, cancellationToken);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}