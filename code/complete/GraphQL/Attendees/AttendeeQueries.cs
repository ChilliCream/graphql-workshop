using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Query")]
    public class AttendeeQueries
    {
        [UseApplicationDbContext]
        [UsePaging]
        [UseFiltering(typeof(AttendeeFilterInputType))]
        public IQueryable<Attendee> GetAttendees(
            [ScopedService] ApplicationDbContext context) => 
            context.Attendees;

        public Task<Attendee> GetAttendeeByIdAsync(
            [ID(nameof(Attendee))]int id,
            AttendeeByIdDataLoader attendeeById,
            CancellationToken cancellationToken) => 
            attendeeById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Attendee>> GetAttendeesByIdAsync(
            [ID(nameof(Attendee))]int[] ids,
            AttendeeByIdDataLoader attendeeById,
            CancellationToken cancellationToken) => 
            await attendeeById.LoadAsync(ids, cancellationToken);
    }
}