using ConferencePlanner.GraphQL.Data;
using GreenDonut.Projections;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Speakers;

[QueryType]
public static class SpeakerQueries
{
    public static async Task<IEnumerable<Speaker>> GetSpeakersAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Speakers.AsNoTracking().ToListAsync(cancellationToken);
    }

    [NodeResolver]
    public static async Task<Speaker?> GetSpeakerByIdAsync(
        int id,
        ISpeakerByIdDataLoader speakerById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await speakerById.Select(selection).LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Speaker>> GetSpeakersByIdAsync(
        [ID<Speaker>] int[] ids,
        ISpeakerByIdDataLoader speakerById,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await speakerById.Select(selection).LoadRequiredAsync(ids, cancellationToken);
    }
}
