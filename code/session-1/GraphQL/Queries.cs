using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL;

public static class Queries
{
    [Query]
    public static IQueryable<Speaker> GetSpeakers(ApplicationDbContext dbContext)
    {
        return dbContext.Speakers;
    }
}
