using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class AttendeeQueries
    {
        /// <summary>
        /// Gets all attendees of this conference.
        /// </summary>
        [UseApplicationDbContext]
        [UsePaging]
        public IQueryable<Attendee> GetAttendees(
            [ScopedService] ApplicationDbContext context) 
            => context.Attendees;

        /// <summary>
        /// Gets an attendee by its identifier.
        /// </summary>
        /// <param name="id">The attendee identifier.</param>
        /// <param name="attendeeById"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Attendee> GetAttendeeByIdAsync(
            [ID(nameof(Attendee))] int id,
            AttendeeByIdDataLoader attendeeById,
            CancellationToken cancellationToken) 
            => attendeeById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Attendee>> GetAttendeesByIdAsync(
            [ID(nameof(Attendee))] int[] ids,
            AttendeeByIdDataLoader attendeeById,
            CancellationToken cancellationToken) 
            => await attendeeById.LoadAsync(ids, cancellationToken);
    }
}