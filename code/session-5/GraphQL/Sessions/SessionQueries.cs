using ConferencePlanner.GraphQL.Data;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

[QueryType]
public static class SessionQueries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Session> GetSessions(ApplicationDbContext dbContext)
    {
        return dbContext.Sessions.AsNoTracking().OrderBy(s => s.Title).ThenBy(s => s.Id);
    }

    [NodeResolver]
    public static async Task<Session?> GetSessionByIdAsync(
        int id,
        ISessionByIdDataLoader sessionById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await sessionById.Select(selection).LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Session>> GetSessionsByIdAsync(
        [ID<Session>] int[] ids,
        ISessionByIdDataLoader sessionById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await sessionById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}
