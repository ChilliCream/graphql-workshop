using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using HotChocolate.AspNetCore;


namespace ConferencePlanner.GraphQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<ApplicationDbContext>(
                options => options.UseSqlite("Data Source=conferences.db"));

            services.AddDataLoader<AttendeeByIdDataLoader>();
            services.AddDataLoader<AttendeeBySessionIdDataLoader>();
            services.AddDataLoader<SessionByAttendeeIdDataLoader>();
            services.AddDataLoader<SessionByIdDataLoader>();
            services.AddDataLoader<SessionBySpeakerIdDataLoader>();
            services.AddDataLoader<SessionByTrackIdDataLoader>();
            services.AddDataLoader<SpeakerByIdDataLoader>();
            services.AddDataLoader<SpeakerBySessionIdDataLoader>();
            services.AddDataLoader<TrackByIdDataLoader>();

            services.AddInMemorySubscriptions();

            services.AddGraphQL(
                SchemaBuilder.New()
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
                    .EnableRelaySupport());
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

            app.UseGraphQL();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
