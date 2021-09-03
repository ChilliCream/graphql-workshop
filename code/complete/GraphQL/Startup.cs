using System;
using System.Collections.Generic;
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
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Resolvers;
using HotChocolate.AspNetCore;
using System.Diagnostics;
using GreenDonut;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Types.Pagination;
using Microsoft.Extensions.Logging;
using IActivityScope = GreenDonut.IActivityScope;

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
                    (s, o) => o
                        .UseSqlite("Data Source=conferences.db")
                        .UseLoggerFactory(s.GetRequiredService<ILoggerFactory>()))
                
                .AddSingleton<IDataLoaderDiagnosticEvents, DataLoaderDiagnostics>()

                // This adds the GraphQL server core service and declares a schema.
                .AddGraphQLServer()
                
                .AddDiagnosticEventListener<HotChocolateDiagnostics>()

                // Next we add the types to our schema.
                .AddQueryType()
                .AddMutationType()
                .AddSubscriptionType()
                
                .AddTypeExtension<AttendeeQueries>()
                .AddTypeExtension<AttendeeMutations>()
                .AddTypeExtension<AttendeeSubscriptions>()
                .AddTypeExtension<AttendeeNode>()
                .AddDataLoader<AttendeeByIdDataLoader>()
                
                .AddTypeExtension<SessionQueries>()
                .AddTypeExtension<SessionMutations>()
                .AddTypeExtension<SessionSubscriptions>()
                .AddTypeExtension<SessionNode>()
                .AddDataLoader<SessionByIdDataLoader>()
                .AddDataLoader<SessionBySpeakerIdDataLoader>()
                
                .AddTypeExtension<SpeakerQueries>()
                .AddTypeExtension<SpeakerMutations>()
                .AddTypeExtension<SpeakerNode>()
                .AddDataLoader<SpeakerByIdDataLoader>()
                .AddDataLoader<SessionBySpeakerIdDataLoader>()
                
                .AddTypeExtension<TrackQueries>()
                .AddTypeExtension<TrackMutations>()
                .AddTypeExtension<TrackNode>()
                .AddDataLoader<TrackByIdDataLoader>()

                // In this section we are adding extensions like relay helpers,
                // filtering and sorting.
                .AddFiltering()
                .AddSorting()
                .AddGlobalObjectIdentification()
                
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

            app.UseCors();

            app.UseWebSockets();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // We will be using the new routing API to host our GraphQL middleware.
                endpoints.MapGraphQL()
                    .WithOptions(new GraphQLServerOptions
                    {
                        Tool =
                        {
                            GaTrackingId = "G-2Y04SFDV8F"
                        }
                    });

                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/graphql", true);
                    return Task.CompletedTask;
                });
            });
        }
    }

    public class DataLoaderDiagnostics : DataLoaderDiagnosticEventListener
    {
        public override IActivityScope ExecuteBatch<TKey>(IDataLoader dataLoader, IReadOnlyList<TKey> keys)
        {
            Console.WriteLine($"{dataLoader.GetType().Name}:{keys.Count}");
            return EmptyScope;
        }
    }

    public class HotChocolateDiagnostics : DiagnosticEventListener
    {
        public override HotChocolate.Execution.Instrumentation.IActivityScope ExecuteRequest(IRequestContext context)
        {
            Console.Clear();
            Console.WriteLine("REQUEST_START");
            Console.WriteLine();
            Console.WriteLine();
            return base.ExecuteRequest(context);
        }
    }
}