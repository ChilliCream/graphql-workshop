using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Mutation")]
    public class AttendeeMutations
    {
        [UseApplicationDbContext]
        public async Task<RegisterAttendeePayload> ScheduleSessionAsync(
            RegisterAttendeeInput input,
            [ScopedService] ApplicationDbContext context)
        {
            var attendee = new Attendee
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                UserName = input.UserName,
                EmailAddress = input.EmailAddress
            };

            context.Attendees.Add(attendee);

            await context.SaveChangesAsync();

            return new RegisterAttendeePayload(attendee, input.ClientMutationId);
        }

        [UseApplicationDbContext]
        public async Task<CheckInAttendeePayload> ScheduleSessionAsync(
            CheckInAttendeeInput input,
            [ScopedService] ApplicationDbContext context)
        {
            Attendee attendee = await context.Attendees.FindAsync(input.AttendeeId);

            if (attendee is null)
            {
                return new CheckInAttendeePayload(
                    new UserError("Attendee not found.", "ATTENDEE_NOT_FOUND"),
                    input.ClientMutationId);
            }

            attendee.SessionsAttendees.Add(
                new SessionAttendee
                {
                    SessionId = input.SessionId
                });

            await context.SaveChangesAsync();

            return new CheckInAttendeePayload(attendee, input.SessionId, input.ClientMutationId);
        }
    }
}