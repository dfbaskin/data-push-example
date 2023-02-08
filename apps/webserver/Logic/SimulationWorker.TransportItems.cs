public sealed partial class SimulationWorker
{
    public const double CenterLatitude = 35.565752687910056;
    public const double CenterLongitude = -83.49854631914549;
    public const double LatitudeOffset = 0.15;
    public const double LongitudeOffset = 0.35;
    private const string HomeOfficeAddress = "Home Office";

    private async Task TransportItems(Context contextBase)
    {
        var manifest = ReceiveManifest();
        var groupAssignment = GetGroupAssignment();
        var (driver, vehicle) = await PickDriverAndVehicle(contextBase);
        var transport = Transport.CreateTransport(manifest);
        var context = new TransportContext(
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

    private async Task<(Driver, Vehicle)> PickDriverAndVehicle(Context context)
    {
        var driverTask = PickDriver(context);
        var vehicleTask = PickVehicle(context);
        await Task.WhenAll(driverTask, vehicleTask);
        return (driverTask.Result, vehicleTask.Result);
    }

    private async Task<Driver> PickDriver(Context context)
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

    private async Task<Vehicle> PickVehicle(Context context)
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

    private async Task<TransportContext> LoadTransport(TransportContext context)
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
                    Location = new Location(
                        Latitude: null,
                        Longitude: null,
                        Address: HomeOfficeAddress
                    )
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

    private async Task<TransportContext> MakeRun(TransportContext context)
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
                        Location = new Location(
                            Latitude: homeLocation.Lat + (secs * latMultiplier),
                            Longitude: homeLocation.Lng + (secs * lngMultiplier),
                            Address: null
                        )
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
                        Location = new Location(
                            Latitude: destLocation.Lat - (secs * latMultiplier),
                            Longitude: destLocation.Lng - (secs * lngMultiplier),
                            Address: null
                        )
                    };
                })
                .Update(context);

            await Task.Delay(1000);
        }

        context = await FinishTransport(context, homeLocation);

        return context;
    }

    private async Task<TransportContext> BeginTransport(
        TransportContext context,
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
                    Location = new Location(
                        Latitude: homeLocation.Lat,
                        Longitude: homeLocation.Lng,
                        Address: HomeOfficeAddress
                    )
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

    private async Task<TransportContext> UnloadTransport(
        TransportContext context,
        (double Lat, double Lng) destLocation
    )
    {
        var addr = Faker.Address.StreetAddress();
        context = await UpdateVehicle()
            .Modify(vehicle =>
            {
                return vehicle with
                {
                    Location = new Location(
                        Latitude: destLocation.Lat,
                        Longitude: destLocation.Lng,
                        Address: addr
                    )
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
            .AddHistory($"Arrived at destination, unloading.")
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

    private async Task<TransportContext> FinishTransport(
        TransportContext context,
        (double Lat, double Lng) homeLocation
    )
    {
        context = await UpdateTransport()
            .Modify(transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Finished
                };
            })
            .AddHistory($"Finished transport.")
            .Update(context);
        context = await UpdateVehicle()
            .Modify(vehicle =>
            {
                return vehicle with
                {
                    Status = VehicleStatus.Available,
                    Location = new Location(
                        Latitude: homeLocation.Lat,
                        Longitude: homeLocation.Lng,
                        Address: HomeOfficeAddress
                    )
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

    private record TransportContext(
        CancellationToken Token,
        string GroupAssignment,
        Transport Transport,
        Driver Driver,
        Vehicle Vehicle
    ) : Context(
        Token: Token
    )
    {
        public string DriverId => Driver.DriverId;
        public string VehicleId => Vehicle.VehicleId;
        public string TransportId => Transport.TransportId;
        public Manifest Manifest => Transport.Manifest;
    }
}
