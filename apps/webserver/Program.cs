var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<GroupQueryExtensions>();

builder.Services
    .AddSingleton<CurrentData>()
    .AddHostedService<SimulationWorker>();

var app = builder.Build();

app.MapGet("/api/ping", () => new {
  timestamp = DateTime.UtcNow
});

app.MapGraphQL();

await app.RunAsync();
