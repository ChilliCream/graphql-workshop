using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Data.Sorting;

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
                    options => options.UseSqlite("Data Source=conferences.db"))

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

                    // In this section we are adding extensions like relay helpers,
                    // filtering and sorting.
                    .AddFiltering()
                    .AddSorting()
                    .EnableRelaySupport()

                    // Now we add some the DataLoader to our system. 
                    .AddDataLoader<AttendeeByIdDataLoader>()
                    .AddDataLoader<SessionByIdDataLoader>()
                    .AddDataLoader<SpeakerByIdDataLoader>()
                    .AddDataLoader<TrackByIdDataLoader>()

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

            app.UseWebSockets();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // We will be using the new routing API to host our GraphQL middleware.
                endpoints.MapGraphQL();
                // endpoints.MapBananaCakePop(); // this one is coming later.

                endpoints.MapGet("/", context =>
                {
                    // context.Response.Redirect("/playground");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
