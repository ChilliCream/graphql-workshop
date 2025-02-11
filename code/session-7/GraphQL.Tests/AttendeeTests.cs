using ConferencePlanner.GraphQL.Data;
using CookieCrumble;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace GraphQL.Tests;

public sealed class AttendeeTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17.2")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4")
        .Build();

    private IRequestExecutor _requestExecutor = null!;

    public async ValueTask InitializeAsync()
    {
        // Start test containers.
        await Task.WhenAll(_postgreSqlContainer.StartAsync(), _redisContainer.StartAsync());

        // Build request executor.
        _requestExecutor = await new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(_postgreSqlContainer.GetConnectionString()))
            .AddGraphQLServer()
            .AddGlobalObjectIdentification()
            .AddMutationConventions()
            .AddDbContextCursorPagingProvider()
            .AddPagingArguments()
            .AddFiltering()
            .AddSorting()
            .AddRedisSubscriptions(
                _ => ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString()))
            .AddGraphQLTypes()
            .BuildRequestExecutorAsync();

        // Create database.
        var dbContext = _requestExecutor.Services
            .GetApplicationServices()
            .GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task RegisterAttendee()
    {
        // Arrange & act
        var result = await _requestExecutor.ExecuteAsync(
            """
            mutation RegisterAttendee {
                registerAttendee(
                    input: {
                        firstName: "Michael"
                        lastName: "Staib"
                        username: "michael"
                        emailAddress: "michael@chillicream.com"
                    }
                ) {
                    attendee {
                        id
                    }
                }
            }
            """,
            TestContext.Current.CancellationToken);

        // Assert
        result.MatchSnapshot(extension: ".json");
    }

    public async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }
}
