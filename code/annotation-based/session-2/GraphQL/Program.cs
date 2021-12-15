using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPooledDbContextFactory<ApplicationDbContext>(
        options => options.UseSqlite("Data Source=conferences.db"));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<SessionSpeakerResolvers>()
    .AddMutationConventions()
    .AddDataLoader<SpeakerByIdDataLoader>()
    .AddDataLoader<SessionByIdDataLoader>()
    .RegisterDbContext<ApplicationDbContext>(kind: DbContextKind.Pooled);

var app = builder.Build();

app.MapGraphQL();

app.Run();
