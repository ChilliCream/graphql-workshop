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
