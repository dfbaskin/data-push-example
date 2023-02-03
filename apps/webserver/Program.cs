var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AllowIntrospection(false)
    .AddQueryType<Query>()
    .AddTypeExtension<GroupExtensions>()
    .AddTypeExtension<DriverExtensions>()
    .AddTypeExtension<VehicleExtensions>()
    .AddTypeExtension<TransportExtensions>();

builder.Services
    .AddSingleton<CurrentData>()
    .AddHostedService<SimulationWorker>();

var app = builder.Build();

app.MapGet("/api/ping", () => new {
  timestamp = DateTime.UtcNow
});

app.MapGraphQL();

await app.RunAsync();
