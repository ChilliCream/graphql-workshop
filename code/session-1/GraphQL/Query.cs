using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL;

public class Query
{
    public IQueryable<Speaker> GetSpeakers(ApplicationDbContext context) =>
        context.Speakers;
}