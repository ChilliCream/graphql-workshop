using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.GraphQL;

[ExtendObjectType(typeof(Speaker))]
public class SessionSpeakerResolvers
{
    [BindMember(nameof(Speaker.Sessions))]
    public IQueryable<Session> GetSessions(
        [Parent] Speaker speaker, 
        ApplicationDbContext context) 
        => context.Speakers
            .Where(t => t.Id == speaker.Id)
            .Include(t => t.Sessions)
            .SelectMany(t => t.Sessions);
}