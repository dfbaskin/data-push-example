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

        context = await LoadTransport(context);
        context = await MakeRun(context);
    }

    private Manifest ReceiveManifest()
    {
        var manifest = Manifest.CreateManifest();
        Logger.LogInformation("Received manifest with {count} items.", manifest.Items.Count);
        return manifest;
    }

    private string GetGroupAssignment()
    {
        var groupAssignment = Current.Groups.Select(g => g.Name).PickOneOf();
        Logger.LogInformation("Using group assignment: {groupAssignment}.", groupAssignment);
        return groupAssignment;
    }

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
                var driverId = available.PickOneOf();
                var driver = Current.UpdateDriver(driverId, (d) =>
                {
                    return d with
                    {
                        Status = DriverStatus.Active,
                        History = d.History.AppendItem(
                            HistoryEntry.CreateHistoryEntry("Entered Active status.")
                        )
                    };
                });
                if (driver != null)
                {
                    Logger.LogInformation("Available driver found: {driverId}.", driver.DriverId);
                    return driver;
                }
            }

            await WaitForAFewSeconds(context, "Available driver not found, waiting.");
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
                var vehicleId = available.PickOneOf();
                var vehicle = Current.UpdateVehicle(vehicleId, (v) =>
                {
                    return v with
                    {
                        Status = VehicleStatus.Active,
                        History = v.History.AppendItem(
                            HistoryEntry.CreateHistoryEntry("Entered Active status.")
                        )
                    };
                });
                if (vehicle != null)
                {
                    Logger.LogInformation("Available vehicle found: {vehicleId}.", vehicle.VehicleId);
                    return vehicle;
                }
            }

            await WaitForAFewSeconds(context, "Available vehicle not found, waiting.");
        }
    }

    private async Task<TransportContext> LoadTransport(TransportContext context)
    {
        Current.AddTransport(context.Transport);
        Logger.LogInformation("Created Transport {transportId}.", context.TransportId);

        context = UpdateDriver(
            context,
            driver =>
            {
                return driver with
                {
                    GroupAssignment = context.GroupAssignment
                };
            },
            $"Driver assigned to group {context.GroupAssignment}.",
            $"Driver assigned to vehicle {context.VehicleId}."
        );
        context = UpdateVehicle(
            context,
            vehicle => {
                return vehicle with {
                    Location = new Location(
                        Latitude: null,
                        Longitude: null,
                        Address: HomeOfficeAddress
                    )
                };
            },
            $"Vehicle assigned to driver {context.DriverId}."
        );
        context = UpdateTransport(
            context,
            transport => {
                return transport with {
                    Status = TransportStatus.Loading,
                    DriverId = context.DriverId,
                    VehicleId = context.VehicleId,
                };
            },
            $"Received manifest with {context.Manifest.Items.Count} items.",
            $"Assigned to driver/vehicle {context.DriverId}/{context.VehicleId}."
        );

        await WaitForAFewSeconds(context, "Loading transport.");
        return context;
    }

    private async Task<TransportContext> MakeRun(TransportContext context)
    {
        var homeLocation = InitialLocation();
        var destLocation = RandomLocation();
        var pace = 0.01;
        var distance = Math.Max(
            Math.Abs(homeLocation.Lat - destLocation.Lat),
            Math.Abs(homeLocation.Lng - destLocation.Lng)
        );
        var totalSeconds = distance / pace;
        var latMultiplier = (homeLocation.Lat - destLocation.Lat) / totalSeconds;
        var lngMultiplier = (homeLocation.Lng - destLocation.Lng) / totalSeconds;

        context = await BeginTransport(context, homeLocation);

        for (int secs = 0; secs < totalSeconds; secs++)
        {
            context = UpdateVehicle(context, vehicle =>
            {
                return vehicle with
                {
                    Location = new Location(
                        Latitude: homeLocation.Lat + (secs * latMultiplier),
                        Longitude: homeLocation.Lng + (secs * lngMultiplier),
                        Address: null
                    )
                };
            });

            await Task.Delay(1000);
        }

        context = await UnloadTransport(context, destLocation);

        for (int secs = 0; secs < totalSeconds; secs++)
        {
            context = UpdateVehicle(context, vehicle =>
            {
                return vehicle with
                {
                    Location = new Location(
                        Latitude: destLocation.Lat - (secs * latMultiplier),
                        Longitude: destLocation.Lng - (secs * lngMultiplier),
                        Address: null
                    )
                };
            });

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
        context = UpdateTransport(
            context,
            transport =>
            {
                return transport with
                {
                    Status = TransportStatus.InRoute
                };
            },
            $"Loaded, beginning run.");
        context = UpdateVehicle(
            context,
            vehicle =>
            {
                return vehicle with
                {
                    Location = new Location(
                        Latitude: homeLocation.Lat,
                        Longitude: homeLocation.Lng,
                        Address: HomeOfficeAddress
                    )
                };
            },
            $"Beginning transport run ({context.TransportId})."
        );
        context = UpdateDriver(
            context,
            $"Beginning transport run ({context.TransportId})."
        );

        await WaitForAFewSeconds(context, "Beginning transport.");
        Logger.LogInformation("Beginning transport run.");
        return context;
    }

    private async Task<TransportContext> UnloadTransport(
        TransportContext context,
        (double Lat, double Lng) destLocation
    )
    {
        var addr = Faker.Address.StreetAddress();
        context = UpdateVehicle(
            context,
            vehicle =>
            {
                return vehicle with
                {
                    Location = new Location(
                        Latitude: destLocation.Lat,
                        Longitude: destLocation.Lng,
                        Address: addr
                    )
                };
            },
            $"Arrived at destination for transport run ({context.TransportId} - {addr})."
        );
        context = UpdateTransport(
            context,
            transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Unloading
                };
            }
        );

        await WaitForAFewSeconds(context, "Unloading transport.");

        context = UpdateTransport(
            context,
            transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Returning
                };
            }
        );

        await WaitForAFewSeconds(context, "Returning home.");

        return context;
    }

    private async Task<TransportContext> FinishTransport(
        TransportContext context,
        (double Lat, double Lng) homeLocation
    )
    {
        context = UpdateTransport(
            context,
            transport =>
            {
                return transport with
                {
                    Status = TransportStatus.Finished
                };
            },
            $"Finished transport.");
        context = UpdateVehicle(
            context,
            vehicle =>
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
            },
            $"Finished transport run ({context.TransportId})."
        );
        context = UpdateDriver(
            context,
            driver =>
            {
                return driver with
                {
                    Status = DriverStatus.Available,
                    GroupAssignment = null
                };
            },
            $"Finished transport run ({context.TransportId})."
        );

        await WaitForAFewSeconds(context, "Finishing transport.");
        Logger.LogInformation("Finished transport run.");
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
