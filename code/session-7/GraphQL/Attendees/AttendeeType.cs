using ConferencePlanner.GraphQL.Data;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;

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
                    => await ctx.DataLoader<IAttendeeByIdDataLoader>()
                        .LoadAsync(id, ctx.RequestAborted));
    }

    [BindMember(nameof(Attendee.SessionsAttendees))]
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Attendee attendee,
        ISessionsByAttendeeIdDataLoader sessionsByAttendeeId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await sessionsByAttendeeId
            .Select(selection)
            .LoadRequiredAsync(attendee.Id, cancellationToken);
    }
}
