using ConferencePlanner.GraphQL.Data;
using GreenDonut.Data;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL;

public static class Queries
{
    [Query]
    public static async Task<IEnumerable<Speaker>> GetSpeakersAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Speakers.AsNoTracking().ToListAsync(cancellationToken);
    }

    [Query]
    public static async Task<Speaker?> GetSpeakerAsync(
        int id,
        ISpeakerByIdDataLoader speakerById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await speakerById.Select(selection).LoadAsync(id, cancellationToken);
    }
}
