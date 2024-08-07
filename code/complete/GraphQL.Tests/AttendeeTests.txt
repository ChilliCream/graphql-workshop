using System;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL;
using ConferencePlanner.GraphQL.Attendees;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;
using ConferencePlanner.GraphQL.Speakers;
using ConferencePlanner.GraphQL.Tracks;
using ConferencePlanner.GraphQL.Types;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace GraphQL.Tests
{
    public class AttendeeTests
    {
        [Fact]
        public async Task Attendee_Schema_Changed()
        {
            ISchema schema =
                await new ServiceCollection()
                    .AddDbContextPool<ApplicationDbContext>(
                        options => options.UseInMemoryDatabase("Data Source=conferences.db"))
                    .AddGraphQL()
                    .AddQueryType(d => d.Name("Query"))
                        .AddTypeExtension<AttendeeQueries>()
                    .AddMutationType(d => d.Name("Mutation"))
                        .AddTypeExtension<AttendeeMutations>()
                    .AddType<AttendeeType>()
                    .AddType<SessionType>()
                    .AddType<SpeakerType>()
                    .AddType<TrackType>()
                    .EnableRelaySupport()
                    .BuildSchemaAsync();

            schema.Print().MatchSnapshot();
        }
        
        [Fact]
        public async Task RegisterAttendee()
        {
            // arrange
            IServiceProvider services = new ServiceCollection()
                .AddDbContextPool<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("Data Source=conferences.db"))
                .AddGraphQL()
                    .AddQueryType(d => d.Name("Query"))
                        .AddTypeExtension<AttendeeQueries>()
                    .AddMutationType(d => d.Name("Mutation"))
                        .AddTypeExtension<AttendeeMutations>()
                    .AddType<AttendeeType>()
                    .AddType<SessionType>()
                    .AddType<SpeakerType>()
                    .AddType<TrackType>()
                    // .EnableRelaySupport()
                .Services
                .BuildServiceProvider();
            
            // act
            IExecutionResult result = await services.ExecuteRequestAsync(
                QueryRequestBuilder.New()
                    .SetQuery(@"
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
                        }")
                    .Create());
            
            // assert
            result.MatchSnapshot();
        }
    }
}
