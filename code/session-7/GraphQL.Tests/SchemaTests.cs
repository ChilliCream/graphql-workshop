using ConferencePlanner.GraphQL.Data;
using CookieCrumble;
using Microsoft.Extensions.DependencyInjection;
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
            .BuildSchemaAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        schema.MatchSnapshot(extension: ".graphql");
    }
}
