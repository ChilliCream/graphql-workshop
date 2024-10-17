using ConferencePlanner.GraphQL.Data;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
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
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await attendeeById.Select(selection).LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Attendee>> GetAttendeesByIdAsync(
        [ID<Attendee>] int[] ids,
        IAttendeeByIdDataLoader attendeeById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await attendeeById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}
