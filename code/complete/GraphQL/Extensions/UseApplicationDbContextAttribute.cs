using ConferencePlanner.GraphQL.Data;
using HotChocolate.Data;

namespace ConferencePlanner.GraphQL
{
    public class UseApplicationDbContextAttribute : UseDbContextAttribute
    {
        public UseApplicationDbContextAttribute() : base(typeof(ApplicationDbContext))
        {
        }
    }
}