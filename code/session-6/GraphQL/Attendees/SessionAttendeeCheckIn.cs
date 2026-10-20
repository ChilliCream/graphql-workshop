using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;

namespace ConferencePlanner.GraphQL.Attendees;

public sealed class SessionAttendeeCheckIn(int attendeeId, int sessionId)
{
    [ID<Attendee>]
    public int AttendeeId { get; } = attendeeId;

    [ID<Session>]
    public int SessionId { get; } = sessionId;

    public async Task<int> CheckInCountAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(s => s.Id == SessionId)
            .SelectMany(s => s.SessionAttendees)
            .CountAsync(cancellationToken);
    }

    public async Task<Attendee> GetAttendeeAsync(
        IAttendeeByIdDataLoader attendeeById,
        CancellationToken cancellationToken)
    {
        return await attendeeById.LoadRequiredAsync(AttendeeId, cancellationToken);
    }

    public async Task<Session> GetSessionAsync(
        ISessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadRequiredAsync(SessionId, cancellationToken);
    }
}
