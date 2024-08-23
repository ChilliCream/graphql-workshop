using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Attendees;

[ObjectType<Attendee>]
public static partial class AttendeeType
{
    static partial void Configure(IObjectTypeDescriptor<Attendee> descriptor)
    {
        descriptor
            .ImplementsNode()
            .IdField(a => a.Id)
            .ResolveNode(
                async (ctx, id)
                    => await ctx.DataLoader<AttendeeByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted));
    }

    [BindMember(nameof(Attendee.SessionsAttendees))]
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Attendee attendee,
        ApplicationDbContext dbContext,
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        var sessionIds = await dbContext.Attendees
            .Where(a => a.Id == attendee.Id)
            .Include(a => a.SessionsAttendees)
            .SelectMany(a => a.SessionsAttendees.Select(sa => sa.SessionId))
            .ToArrayAsync(cancellationToken);

        return await sessionById.LoadRequiredAsync(sessionIds, cancellationToken);
    }
}
