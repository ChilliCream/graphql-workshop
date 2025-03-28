using ConferencePlanner.GraphQL.Data;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL.Attendees;

[MutationType]
public static class AttendeeMutations
{
    public static async Task<Attendee> RegisterAttendeeAsync(
        RegisterAttendeeInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var attendee = new Attendee
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Username = input.Username,
            EmailAddress = input.EmailAddress
        };

        dbContext.Attendees.Add(attendee);

        await dbContext.SaveChangesAsync(cancellationToken);

        return attendee;
    }

    public static async Task<Attendee> CheckInAttendeeAsync(
        CheckInAttendeeInput input,
        ApplicationDbContext dbContext,
        ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var attendee = await dbContext.Attendees.FirstOrDefaultAsync(
            a => a.Id == input.AttendeeId,
            cancellationToken);

        if (attendee is null)
        {
            throw new AttendeeNotFoundException();
        }

        attendee.SessionsAttendees.Add(new SessionAttendee { SessionId = input.SessionId });

        await dbContext.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(
            $"OnAttendeeCheckedIn_{input.SessionId}",
            input.AttendeeId,
            cancellationToken);

        return attendee;
    }
}
