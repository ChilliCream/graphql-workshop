using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(
        options => options.UseSqlite("Data Source=conferences.db"));

builder.Services
    .AddGraphQLServer()
    .AddDocumentFromFile("./schema.graphql")
    .BindRuntimeType<Query>()
    .BindRuntimeType<Mutation>()
    .AddMutationConventions()
    .RegisterDbContext<ApplicationDbContext>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
