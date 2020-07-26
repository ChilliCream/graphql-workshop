using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Query")]
    public class SessionQueries
    {
        [UseApplicationDbContext]
        public async Task<IEnumerable<Session>> GetSessionsAsync(
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken) => 
            await context.Sessions.ToListAsync(cancellationToken);
    }
}