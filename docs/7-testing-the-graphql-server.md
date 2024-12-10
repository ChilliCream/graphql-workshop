# Testing the GraphQL server

- [Adding a schema change test](#adding-a-schema-change-test)
- [Adding a simple query test](#adding-a-simple-query-test)

## Adding a schema change test

A schema change test will simply create a snapshot of your schema, and always fails if the schema changes. This kind of test is often useful when working with pure code-first, where a simple change in C# can create a breaking change in your GraphQL schema.

1. Create an xUnit test project:

    ```shell
    dotnet new xunit --name GraphQL.Tests
    ```

1. Add the project to our solution:

    ```shell
    dotnet sln add GraphQL.Tests
    ```

1. Head over to the `GraphQL.Tests.csproj` file and update the package references to the following:

    ```xml
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    ```

1. Add a reference to the NuGet package `CookieCrumble` version `14.2.0`:
    - `dotnet add GraphQL.Tests package CookieCrumble --version 14.2.0`

1. Add a reference to the GraphQL server:
    - `dotnet add GraphQL.Tests reference GraphQL`

1. Rename the file `UnitTest1.cs` to `SchemaTests.cs` and replace the code with the following:

    ```csharp
    using ConferencePlanner.GraphQL.Data;
    using Microsoft.Extensions.DependencyInjection;
    using CookieCrumble;
    using HotChocolate.Execution;

    namespace GraphQL.Tests;

    public sealed class SchemaTests
    {
        [Fact]
        public async Task SchemaChanged()
        {
            // Arrange & act
            var schema = await new ServiceCollection()
                .AddDbContext<ApplicationDbContext>()
                .AddGraphQLServer()
                .AddGlobalObjectIdentification()
                .AddMutationConventions()
                .AddDbContextCursorPagingProvider()
                .AddPagingArguments()
                .AddFiltering()
                .AddSorting()
                .AddInMemorySubscriptions()
                .AddGraphQLTypes()
                .BuildSchemaAsync();

            // Assert
            schema.MatchSnapshot();
        }
    }
    ```

    The above test takes the service collection and builds a schema from it. We call `MatchSnapshot` to create a snapshot of the GraphQL SDL representation of the schema, which is compared in subsequent test runs.

## Adding a simple query test

1. Add a reference to the following NuGet packages:
    - `Testcontainers.PostgreSql` version `4.1.0`:
      - `dotnet add GraphQL.Tests package Testcontainers.PostgreSql --version 4.1.0`
    - `Testcontainers.Redis` version `4.1.0`:
      - `dotnet add GraphQL.Tests package Testcontainers.Redis --version 4.1.0`

1. Add a new class named `AttendeeTests.cs`:

    ```csharp
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

        public async Task InitializeAsync()
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
                """);

            // Assert
            result.MatchSnapshot(extension: ".json");
        }

        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();
        }
    }
    ```

    In the above test, we use [Testcontainers](https://dotnet.testcontainers.org/) for PostgreSQL and Redis, for realistic integration testing, as opposed to using in-memory providers.

    To execute against a schema we can call `BuildRequestExecutorAsync` on the service collection to get an `IRequestExecutor`. After executing the mutation, we snapshot the result object, and as with the previous test, subsequent test runs will compare our snapshot file.

[**<< Session #6 - Adding real-time functionality with subscriptions**](6-subscriptions.md)
