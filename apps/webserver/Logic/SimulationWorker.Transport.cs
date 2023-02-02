public sealed partial class SimulationWorker
{
    private async Task TransportItems(Context context)
    {
        var groupAssignment = PickGroupAssignment();

        var driver = await PickDriver(context, groupAssignment);
    }

    private string PickGroupAssignment()
    {
        var groupAssignment = Current.Groups.Select(g => g.Name).PickOneOf();
        Logger.LogInformation("Using group assignment: {groupAssignment}.", groupAssignment);
        return groupAssignment;
    }

    private async Task<Driver> PickDriver(Context context, string groupAssignment)
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
                        GroupAssignment = groupAssignment,
                        Status = DriverStatus.Active,
                    };
                });
                if (driver != null)
                {
                    Logger.LogInformation("Available driver found: {driverId}.", driver.DriverId);
                    return driver;
                }
            }

            Logger.LogInformation("Available driver not found, waiting.");
            await Task.Delay(1000, context.Token);
        }
    }
}
