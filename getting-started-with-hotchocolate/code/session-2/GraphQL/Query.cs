using System.Linq;
using HotChocolate;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL
{
    public class Query
    {
        public IQueryable<Speaker> GetSpeakers([Service] ApplicationDbContext context) =>
            context.Speakers;
    }
}