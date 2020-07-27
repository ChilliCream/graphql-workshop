using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Query")]
    public class AttendeeQueries
    {
        [UseApplicationDbContext]
        public async Task<IEnumerable<Attendee>> GetAttendeesAsync(
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken) => 
            await context.Attendees.ToListAsync(cancellationToken);

        public Task<Attendee> GetAttendeeByIdAsync(
            [ID(nameof(Attendee))]int id,
            AttendeeByIdDataLoader sessionById,
            CancellationToken cancellationToken) => 
            sessionById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Attendee>> GetAttendeesByIdAsync(
            [ID(nameof(Attendee))]int[] ids,
            AttendeeByIdDataLoader sessionById,
            CancellationToken cancellationToken) => 
            await sessionById.LoadAsync(ids, cancellationToken);
    }
}