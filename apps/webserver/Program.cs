var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddSubscriptionType<Subscription>()
    .AddTypeExtension<GroupExtensions>()
    .AddTypeExtension<DriverExtensions>()
    .AddTypeExtension<VehicleExtensions>()
    .AddTypeExtension<TransportExtensions>()
    .AddInMemorySubscriptions();

builder.Services
    .AddSingleton<CurrentData>()
    .AddHostedService<SimulationWorker>();

var app = builder.Build();

app.MapGet("/api/ping", () => new {
  timestamp = DateTime.UtcNow
});

app.UseWebSockets();

app.MapGraphQL();

await app.RunAsync();
