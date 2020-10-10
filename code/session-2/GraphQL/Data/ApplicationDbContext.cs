using Microsoft.EntityFrameworkCore;

    namespace ConferencePlanner.GraphQL.Data
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Speaker> Speakers { get; set; } = default!;
        }
    }