using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Speakers;

[ObjectType<Speaker>]
public static partial class SpeakerType
{
    static partial void Configure(IObjectTypeDescriptor<Speaker> descriptor)
    {
        descriptor.Ignore(s => s.SessionSpeakers);
    }

    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Speaker speaker,
        ApplicationDbContext dbContext,
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        var sessionIds = await dbContext.Speakers
            .Where(s => s.Id == speaker.Id)
            .Include(s => s.SessionSpeakers)
            .SelectMany(s => s.SessionSpeakers.Select(ss => ss.SessionId))
            .ToArrayAsync(cancellationToken);

        return await sessionById.LoadAsync(sessionIds, cancellationToken);
    }
}
