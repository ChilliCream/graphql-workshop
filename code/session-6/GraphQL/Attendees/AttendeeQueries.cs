using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Attendees;

[QueryType]
public static class AttendeeQueries
{
    [UsePaging]
    public static IQueryable<Attendee> GetAttendees(ApplicationDbContext dbContext)
    {
        return dbContext.Attendees.AsNoTracking().OrderBy(a => a.Username);
    }

    [NodeResolver]
    public static async Task<Attendee?> GetAttendeeByIdAsync(
        int id,
        IAttendeeByIdDataLoader attendeeById,
        CancellationToken cancellationToken)
    {
        return await attendeeById.LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Attendee>> GetAttendeesByIdAsync(
        [ID<Attendee>] int[] ids,
        IAttendeeByIdDataLoader attendeeById,
        CancellationToken cancellationToken)
    {
        return await attendeeById.LoadRequiredAsync(ids, cancellationToken);
    }
}
