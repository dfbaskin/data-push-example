var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGet("/api/ping", () => new {
  timestamp = DateTime.UtcNow
});

app.MapGraphQL();

await app.RunAsync();
