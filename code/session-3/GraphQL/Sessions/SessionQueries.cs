using ConferencePlanner.GraphQL.Data;
using GreenDonut.Projections;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

[QueryType]
public static class SessionQueries
{
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions.AsNoTracking().ToListAsync(cancellationToken);
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
