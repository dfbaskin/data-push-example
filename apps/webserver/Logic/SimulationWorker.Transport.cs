public sealed partial class SimulationWorker
{
    private async Task TransportItems(Context contextBase)
    {
        var manifest = ReceiveManifest();
        var groupAssignment = GetGroupAssignment();

        var (driver, vehicle) = await PickDriverAndVehicle(contextBase);

        var transport = Transport.CreateTransport(manifest);
        Logger.LogInformation("Created Transport {transportId}.", transport.TransportId);

        Current.AddTransport(transport);
        var context = new TransportContext(
            Token: contextBase.Token,
            Transport: transport,
            Driver: driver,
            Vehicle: vehicle
        );

        context = UpdateDriver(context, $"Driver assigned to vehicle {vehicle.VehicleId}.");
        context = UpdateVehicle(context, $"Vehicle assigned to driver {driver.DriverId}.");
        context = UpdateTransport(context, $"Received manifest with {manifest.Items.Count} items.");
        context = UpdateTransport(context, $"Assigned to driver/vehicle {driver.DriverId}/{vehicle.VehicleId}.");



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

    private async Task WaitForAFewSeconds(Context context, string message)
    {
        int seconds = Faker.RandomNumber.Next(3, 7);
        await Task.Delay(seconds * 1000, context.Token);
    }

    private record TransportContext(
        CancellationToken Token,
        Transport Transport,
        Driver Driver,
        Vehicle Vehicle
    ) : Context(
        Token: Token
    );
}
