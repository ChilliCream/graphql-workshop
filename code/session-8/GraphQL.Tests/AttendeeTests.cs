using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Execution;
using Snapshooter.Xunit;
using Xunit;

namespace GraphQL.Tests
{
    public class AttendeeTests
    {
        [Fact]
        public async Task Attendee_Schema_Changed()
        {
            // arrange
            // act
            ISchema schema = await new ServiceCollection()
                .AddPooledDbContextFactory<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("Data Source=conferences.db"))
                .AddGraphQL()
                .AddQueryType(d => d.Name("Query"))
                    .AddType<AttendeeQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                    .AddType<AttendeeMutations>()
                .AddType<AttendeeType>()
                .AddType<SessionType>()
                .AddType<SpeakerType>()
                .AddType<TrackType>()
                .EnableRelaySupport()
                .BuildSchemaAsync();

            // assert
            schema.Print().MatchSnapshot();
        }

        [Fact]
        public async Task RegisterAttendee()
        {
            // arrange
            IRequestExecutor executor = await new ServiceCollection()
                .AddPooledDbContextFactory<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("Data Source=conferences.db"))
                .AddGraphQL()
               .AddQueryType(d => d.Name("Query"))
                   .AddType<AttendeeQueries>()
               .AddMutationType(d => d.Name("Mutation"))
                   .AddType<AttendeeMutations>()
               .AddType<AttendeeType>()
               .AddType<SessionType>()
               .AddType<SpeakerType>()
               .AddType<TrackType>()
               .EnableRelaySupport()
               .BuildRequestExecutorAsync();

            // act
            IExecutionResult result = await executor.ExecuteAsync(@"
                mutation RegisterAttendee {
                    registerAttendee(
                        input: {
                            emailAddress: ""michael@chillicream.com""
                                firstName: ""michael""
                                lastName: ""staib""
                                userName: ""michael3""
                            })
                    {
                        attendee {
                            id
                        }
                    }
                }");

            // assert
            result.ToJson().MatchSnapshot();
        }
    }
}