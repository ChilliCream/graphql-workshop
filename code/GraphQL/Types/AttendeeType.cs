using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Types
{
    public class AttendeeType : ObjectType<Attendee>
    {
        protected override void Configure(IObjectTypeDescriptor<Attendee> descriptor)
        {
            descriptor
                .AsNode() // todo : maybe we should move the extension to HotChocolate.Types or move it back to relay
                .IdField(t => t.Id)
                .NodeResolver((ctx, id) =>
                    ctx.DataLoader<AttendeeByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));
                    
            descriptor
                .Field(t => t.SessionsAttendees)
                .ResolveWith<AttendeeResolvers>(t => t.GetSessionsAsync(default!, default!, default))
                .Name("sessions");
        }

        private class AttendeeResolvers
        {
            public async Task<IEnumerable<Session>> GetSessionsAsync(
                Attendee attendee,
                SessionByAttendeeIdDataLoader sessionByAttendeeId,
                CancellationToken cancellationToken) =>
                await sessionByAttendeeId.LoadAsync(attendee.Id, cancellationToken);
        }
    }
}