using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using ConferencePlanner.GraphQL.Imports;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using HotChocolate;

namespace ConferencePlanner.GraphQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors(o => 
                    o.AddDefaultPolicy(b => 
                        b.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin()))

                // First we add the DBContext which we will be using to interact with our
                // Database.
                .AddPooledDbContextFactory<ApplicationDbContext>(
                    options => options.UseSqlite("Data Source=conferences.db"))

                // This adds the GraphQL server core service and declares a schema.
                .AddGraphQLServer()

                    // Next we add the types to our schema.
                    .AddQueryType()
                        .AddTypeExtension<AttendeeQueries>()
                        .AddTypeExtension<SessionQueries>()
                        .AddTypeExtension<SpeakerQueries>()
                        .AddTypeExtension<TrackQueries>()
                    .AddMutationType()
                        .AddTypeExtension<AttendeeMutations>()
                        .AddTypeExtension<SessionMutations>()
                        .AddTypeExtension<SpeakerMutations>()
                        .AddTypeExtension<TrackMutations>()
                    .AddSubscriptionType()
                        .AddTypeExtension<AttendeeSubscriptions>()
                        .AddTypeExtension<SessionSubscriptions>()
                    .AddTypeExtension<AttendeeExtensions>()
                    .AddTypeExtension<SessionExtensions>()
                    .AddTypeExtension<TrackExtensions>()
                    .AddTypeExtension<SpeakerExtensions>()

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

                    // we make sure that the db exists and prefill it with conference data.
                    .EnsureDatabaseIsCreated()

                    // Since we are using subscriptions, we need to register a pub/sub system.
                    // for our demo we are using a in-memory pub/sub system.
                    .AddInMemorySubscriptions()

                    // Last we add support for persisted queries. 
                    // The first line adds the persisted query storage, 
                    // the second one the persisted query processing pipeline.
                    .AddFileSystemQueryStorage("./persisted_queries")
                    .UsePersistedQueryPipeline()
                    .InitializeOnStartup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

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
