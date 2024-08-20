using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(
        options => options.UseNpgsql("Host=127.0.0.1;Username=graphql_workshop;Password=secret"))
    .AddGraphQLServer()
    .AddGlobalObjectIdentification()
    .AddMutationConventions()
    .AddFiltering()
    .AddSorting()
    .AddRedisSubscriptions(_ => ConnectionMultiplexer.Connect("127.0.0.1:6379"))
    .AddGraphQLTypes();

var app = builder.Build();

app.UseWebSockets();
app.MapGraphQL();

await app.RunWithGraphQLCommandsAsync(args);
