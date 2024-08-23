using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Types;

[ObjectType<Speaker>]
public static partial class SpeakerType
{
    [BindMember(nameof(Speaker.SessionSpeakers))]
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

        return await sessionById.LoadRequiredAsync(sessionIds, cancellationToken);
    }
}
