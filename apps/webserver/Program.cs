using Microsoft.Extensions.FileProviders;

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
    .AddSingleton<ModelInstanceUpdaterContext>()
    .AddHostedService<SimulationWorker>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    var fileProvider = new PhysicalFileProvider(
        "/Dev/DFB/EventSourcingExample/data-push-example/dist/apps/webapp"
    );

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = fileProvider
    });

    var options = new StaticFileOptions
    {
        FileProvider = fileProvider
    };
    app.UseStaticFiles(options);
    app.UseSpaStaticFiles(options);
}

app.MapGet("/api/ping", () => new {
  timestamp = DateTime.UtcNow
});

app.UseWebSockets();

app.MapGraphQL();

await app.RunAsync();
