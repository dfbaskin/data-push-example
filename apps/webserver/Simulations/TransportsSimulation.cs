public sealed class TransportsSimulation
{
    public TransportsSimulation(
        ModelInstanceUpdaterContext modelContext,
        ILogger<TransportsSimulation> logger
    )
    {
        ModelContext = modelContext ?? throw new ArgumentNullException(nameof(modelContext));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ModelInstanceUpdaterContext ModelContext { get; }
    public CurrentData Current => ModelContext.Current;
    public ILogger<TransportsSimulation> Logger { get; }

    public const double CenterLatitude = 35.565752687910056;
    public const double CenterLongitude = -83.49854631914549;
    public const double LatitudeOffset = 0.15;
    public const double LongitudeOffset = 0.35;
    private const string HomeOfficeAddress = "Home Office";

    private DriverInstanceUpdater UpdateDriver()
        => new DriverInstanceUpdater(ModelContext);
    private VehicleInstanceUpdater UpdateVehicle()
        => new VehicleInstanceUpdater(ModelContext);
    private TransportInstanceUpdater UpdateTransport()
        => new TransportInstanceUpdater(ModelContext);

    public async Task RunSimulation(SimulationContext contextBase)
    {
        var manifest = ReceiveManifest();
        var groupAssignment = GetGroupAssignment();
        var (driver, vehicle) = await PickDriverAndVehicle(contextBase);
        var transport = Transport.CreateTransport(manifest);
        var context = new TransportInstanceContext(
            Token: contextBase.Token,
            GroupAssignment: groupAssignment,
            Transport: transport,
            Driver: driver,
            Vehicle: vehicle
        );

        Logger.LogInformation(
            "Beginning transport run ({driverId}/{vehicleId}/{transportId} -- {itemCount} items).",
            context.DriverId,
            context.VehicleId,
            context.TransportId,
            context.Manifest.Items.Count
        );

        context = await LoadTransport(context);
        context = await MakeRun(context);

        Logger.LogInformation(
            "Finished transport run ({driverId}/{vehicleId}/{transportId} -- {itemCount} items).",
            context.DriverId,
            context.VehicleId,
            context.TransportId,
            context.Manifest.Items.Count
        );
    }

    private Manifest ReceiveManifest() => Manifest.CreateManifest();

    private string GetGroupAssignment() => Current.Groups.Select(g => g.Name).PickOneOf();

    private async Task<(Driver, Vehicle)> PickDriverAndVehicle(SimulationContext context)
    {
        var driverTask = PickDriver(context);
        var vehicleTask = PickVehicle(context);
        await Task.WhenAll(driverTask, vehicleTask);
        return (driverTask.Result, vehicleTask.Result);
    }

    private async Task<Driver> PickDriver(SimulationContext context)
    {
        while (true)
        {
            context.VerifyIsActive();

            var available = Current.Drivers
                .Where(d => d.Status == DriverStatus.Available)
                .Select(d => d.DriverId)
                .ToList();
            if (available.Count > 0)
            {
                var driver = await UpdateDriver()
                    .WithId(available.PickOneOf())
                    .Modify((d) =>
                    {
                        return d with
                        {
                            Status = DriverStatus.Active
                        };
                    })
                    .AddHistory("Entered Active status.")
                    .Update();
                if (driver != null)
                {
                    return driver;
                }
            }

            await WaitForAFewSeconds(context);
        }
    }

    private async Task<Vehicle> PickVehicle(SimulationContext context)
    {
        while (true)
        {
            context.VerifyIsActive();

            var available = Current.Vehicles
                .Where(v => v.Status == VehicleStatus.Available)
                .Select(v => v.VehicleId)
                .ToList();
            if (available.Count > 0)
            {
                var vehicle = await UpdateVehicle()
                    .WithId(available.PickOneOf())
                    .Modify((v) =>
                    {
                        return v with
                        {
                            Status = VehicleStatus.Active
                        };
                    })
                    .AddHistory("Entered Active status.")
                    .Update();
                if (vehicle != null)
                {
                    return vehicle;
                }
            }

            await WaitForAFewSeconds(context);
        }
    }

    private async Task<TransportInstanceContext> LoadTransport(TransportInstanceContext context)
    {
        Current.AddTransport(context.Transport);

        context = await UpdateDriver()
            .Modify(driver =>
            {
                return driver with
                {
                    GroupAssignment = context.GroupAssignment
                };
            })
            .AddHistory($"Driver assigned to group {context.GroupAssignment}.")
            .AddHistory($"Driver assigned to vehicle {context.VehicleId}.")
            .Update(context);

        context = await UpdateVehicle()
            .Modify(vehicle =>
            {
                return vehicle with
                {
                    Location = vehicle.Location with {
                        Latitude = null,
                        Longitude = null,
                        Address = HomeOfficeAddress
                    }
                };
            })
            .AddHistory($"Vehicle assigned to driver {context.DriverId}.")
            .Update(context);

        context = await UpdateTransport()
            .Modify(transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Loading,
                    DriverId = context.DriverId,
                    VehicleId = context.VehicleId,
                };
            })
            .AddHistory($"Received manifest with {context.Manifest.Items.Count} items.")
            .AddHistory($"Assigned to driver/vehicle {context.DriverId}/{context.VehicleId}.")
            .Update(context);

        await WaitForAFewSeconds(context);
        return context;
    }

    private async Task<TransportInstanceContext> MakeRun(TransportInstanceContext context)
    {
        var homeLocation = InitialLocation();
        var destLocation = RandomLocation();
        var pace = 0.01;
        var distance = Math.Max(
            Math.Abs(destLocation.Lat - homeLocation.Lat),
            Math.Abs(destLocation.Lng - homeLocation.Lng)
        );
        var totalSeconds = distance / pace;
        var latMultiplier = (destLocation.Lat - homeLocation.Lat) / totalSeconds;
        var lngMultiplier = (destLocation.Lng - homeLocation.Lng) / totalSeconds;

        context = await BeginTransport(context, homeLocation);

        for (int secs = 0; secs < totalSeconds; secs++)
        {
            context = await UpdateVehicle()
                .Modify(vehicle =>
                {
                    return vehicle with
                    {
                        Location = vehicle.Location with {
                            Latitude = homeLocation.Lat + (secs * latMultiplier),
                            Longitude = homeLocation.Lng + (secs * lngMultiplier),
                            Address = null
                        }
                    };
                })
                .Update(context);

            await Task.Delay(1000);
        }

        context = await UnloadTransport(context, destLocation);

        for (int secs = 0; secs < totalSeconds; secs++)
        {
            context = await UpdateVehicle()
                .Modify(vehicle =>
                {
                    return vehicle with
                    {
                        Location = vehicle.Location with {
                            Latitude = destLocation.Lat - (secs * latMultiplier),
                            Longitude = destLocation.Lng - (secs * lngMultiplier),
                            Address = null
                        }
                    };
                })
                .Update(context);

            await Task.Delay(1000);
        }

        context = await FinishTransport(context, homeLocation);

        return context;
    }

    private async Task<TransportInstanceContext> BeginTransport(
        TransportInstanceContext context,
        (double Lat, double Lng) homeLocation
    )
    {
        context = await UpdateTransport()
            .Modify(transport =>
            {
                return transport with
                {
                    Status = TransportStatus.InRoute
                };
            })
            .AddHistory($"Loaded, beginning run.")
            .Update(context);

        context = await UpdateVehicle()
            .Modify(vehicle =>
            {
                return vehicle with
                {
                    Location = vehicle.Location with {
                        Latitude = homeLocation.Lat,
                        Longitude = homeLocation.Lng,
                        Address = HomeOfficeAddress
                    }
                };
            })
            .AddHistory($"Beginning transport run ({context.TransportId}).")
            .Update(context);

        context = await UpdateDriver()
            .AddHistory($"Beginning transport run ({context.TransportId}).")
            .Update(context);

        await WaitForAFewSeconds(context);
        return context;
    }

    private async Task<TransportInstanceContext> UnloadTransport(
        TransportInstanceContext context,
        (double Lat, double Lng) destLocation
    )
    {
        var addr = Faker.Address.StreetAddress();
        context = await UpdateVehicle()
            .Modify(vehicle =>
            {
                return vehicle with
                {
                    Location = vehicle.Location with {
                        Latitude = destLocation.Lat,
                        Longitude = destLocation.Lng,
                        Address = addr
                    }
                };
            })
            .AddHistory($"Arrived at destination for transport run ({context.TransportId} - {addr}).")
            .Update(context);

        context = await UpdateTransport()
            .Modify(transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Unloading
                };
            })
            .AddHistory($"Arrived at destination ({addr}), unloading.")
            .Update(context);

        await WaitForAFewSeconds(context);

        context = await UpdateTransport()
            .Modify(transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Returning
                };
            })
            .AddHistory($"Finished unloading, returning home.")
            .Update(context);

        await WaitForAFewSeconds(context);

        return context;
    }

    private async Task<TransportInstanceContext> FinishTransport(
        TransportInstanceContext context,
        (double Lat, double Lng) homeLocation
    )
    {
        context = await UpdateVehicle()
            .Modify(vehicle =>
            {
                return vehicle with
                {
                    Status = VehicleStatus.Available,
                    Location = vehicle.Location with {
                        Latitude = homeLocation.Lat,
                        Longitude = homeLocation.Lng,
                        Address = HomeOfficeAddress
                    }
                };
            })
            .AddHistory($"Finished transport run ({context.TransportId}).")
            .Update(context);
        context = await UpdateDriver()
            .Modify(driver =>
            {
                return driver with
                {
                    Status = DriverStatus.Available,
                    GroupAssignment = null
                };
            })
            .AddHistory($"Finished transport run ({context.TransportId}).")
            .Update(context);
        context = await UpdateTransport()
            .Modify(transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Finished,
                    EndTimestampUTC = DateTime.UtcNow,
                };
            })
            .AddHistory($"Finished transport.")
            .Update(context);

        await WaitForAFewSeconds(context);
        return context;
    }

    private (double Lat, double Lng) InitialLocation()
    {
        return (CenterLatitude, CenterLongitude);
    }

    private (double Lat, double Lng) RandomLocation()
    {
        var (lat, lng) = InitialLocation();
        lat += (Random.Shared.NextDouble() * LatitudeOffset) - (LatitudeOffset / 2.0);
        lng += (Random.Shared.NextDouble() * LongitudeOffset) - (LongitudeOffset / 2.0);
        return (lat, lng);
    }

    private async Task WaitForAFewSeconds(SimulationContext context)
    {
        int seconds = Faker.RandomNumber.Next(3, 7);
        await Task.Delay(seconds * 1000, context.Token);
    }
}