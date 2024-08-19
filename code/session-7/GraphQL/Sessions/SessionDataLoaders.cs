using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Sessions;

public static class SessionDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Session>> SessionByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .Where(s => ids.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);
    }
}
