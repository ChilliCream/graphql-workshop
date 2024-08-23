using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers;

[QueryType]
public static class SpeakerQueries
{
    [UsePaging]
    public static IQueryable<Speaker> GetSpeakers(ApplicationDbContext dbContext)
    {
        return dbContext.Speakers.OrderBy(s => s.Name);
    }

    [NodeResolver]
    public static async Task<Speaker?> GetSpeakerByIdAsync(
        int id,
        SpeakerByIdDataLoader speakerById,
        CancellationToken cancellationToken)
    {
        return await speakerById.LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Speaker>> GetSpeakersByIdAsync(
        [ID<Speaker>] int[] ids,
        SpeakerByIdDataLoader speakerById,
        CancellationToken cancellationToken)
    {
        return await speakerById.LoadRequiredAsync(ids, cancellationToken);
    }
}
