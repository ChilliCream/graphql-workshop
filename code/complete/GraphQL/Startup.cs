using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.Imports;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ConferencePlanner.GraphQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                // First we add the DBContext which we will be using to interact with our
                // Database.
                .AddPooledDbContextFactory<ApplicationDbContext>(
                    options => options.UseSqlite("Data Source=conferences.db")
                    .LogTo(Console.WriteLine))

                // This adds the GraphQL server core service and declares a schema.
                .AddGraphQLServer()

                    // Next we add the types to our schema.
                    .AddQueryType(d => d.Name("Query"))
                        .AddType<AttendeeQueries>()
                        .AddType<SessionQueries>()
                        .AddType<SpeakerQueries>()
                        .AddType<TrackQueries>()
                    .AddMutationType(d => d.Name("Mutation"))
                        .AddType<AttendeeMutations>()
                        .AddType<SessionMutations>()
                        .AddType<SpeakerMutations>()
                        .AddType<TrackMutations>()
                    .AddSubscriptionType(d => d.Name("Subscription"))
                        .AddType<AttendeeSubscriptions>()
                        .AddType<SessionSubscriptions>()
                    .AddType<AttendeeType>()
                    .AddType<SessionType>()
                    .AddType<SpeakerType>()
                    .AddType<TrackType>()

                    .AddProjections()
                    // In this section we are adding extensions like relay helpers,
                    // filtering and sorting.
                    .AddFiltering()
                    .AddSorting()
                    .EnableRelaySupport()
                    .SetPagingOptions(new HotChocolate.Types.Pagination.PagingOptions { IncludeTotalCount = true })

                    // Now we add some the DataLoader to our system.
                    .AddDataLoader<AttendeeByIdDataLoader>()
                    .AddDataLoader<SessionByIdDataLoader>()
                    .AddDataLoader<SpeakerByIdDataLoader>()
                    .AddDataLoader<TrackByIdDataLoader>()

                    // we make sure that the db exists and prefill it with conference data.
                    .EnsureDatabaseIsCreated()

                    // Since we are using subscriptions, we need to register a pub/sub system.
                    // for our demo we are using a in-memory pub/sub system.
                    .AddInMemorySubscriptions()

                    // Last we add support for persisted queries.
                    // The first line adds the persisted query storage,
                    // the second one the persisted query processing pipeline.
                    .AddFileSystemQueryStorage("./persisted_queries")
                    .UsePersistedQueryPipeline();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using var ctx = app.ApplicationServices.GetService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();
            //ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            app.UseWebSockets();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // We will be using the new routing API to host our GraphQL middleware.
                endpoints.MapGraphQL();

                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/graphql");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
