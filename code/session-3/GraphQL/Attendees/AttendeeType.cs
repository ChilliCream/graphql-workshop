using ConferencePlanner.GraphQL.Data;

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
        SessionsByAttendeeIdDataLoader sessionsByAttendeeId,
        CancellationToken cancellationToken)
    {
        return await sessionsByAttendeeId.LoadRequiredAsync(attendee.Id, cancellationToken);
    }
}
