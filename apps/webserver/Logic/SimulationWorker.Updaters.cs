public sealed partial class SimulationWorker
{
    private TransportContext UpdateDriver(
        TransportContext context,
        params string[] messages
    ) => UpdateDriver(context, null, messages);

    private TransportContext UpdateDriver(
        TransportContext context,
        Func<Driver, Driver>? updateFn,
        params string[] messages
    )
    {
        var driver = Current.UpdateDriver(context.Driver.DriverId, driver =>
        {
            if (updateFn != null)
            {
                driver = updateFn(driver);
            }
            if (messages.Length > 0)
            {
                driver = driver with
                {
                    History = messages.Aggregate(
                        driver.History,
                        (hist, message) => {
                            return hist.AppendItem(HistoryEntry.CreateHistoryEntry(message));
                        }
                    )
                };
            }
            return driver;
        });
        if (driver == null)
        {
            throw new InvalidOperationException("Could not find driver entity in collection.");
        }
        return context with
        {
            Driver = driver
        };
    }

    private TransportContext UpdateVehicle(
        TransportContext context,
        params string[] messages
    ) => UpdateVehicle(context, null, messages);

    private TransportContext UpdateVehicle(
        TransportContext context,
        Func<Vehicle, Vehicle>? updateFn,
        params string[] messages
    )
    {
        var vehicle = Current.UpdateVehicle(context.Vehicle.VehicleId, vehicle =>
        {
            if (updateFn != null)
            {
                vehicle = updateFn(vehicle);
            }
            if (messages.Length > 0)
            {
                vehicle = vehicle with
                {
                    History = messages.Aggregate(
                        vehicle.History,
                        (hist, message) => {
                            return hist.AppendItem(HistoryEntry.CreateHistoryEntry(message));
                        }
                    )
                };
            }
            return vehicle;
        });
        if (vehicle == null)
        {
            throw new InvalidOperationException("Could not find vehicle entity in collection.");
        }
        return context with
        {
            Vehicle = vehicle
        };
    }

    private TransportContext UpdateTransport(
        TransportContext context,
        params string[] messages
    ) => UpdateTransport(context, null, messages);

    private TransportContext UpdateTransport(
        TransportContext context,
        Func<Transport, Transport>? updateFn,
        params string[] messages
    )
    {
        var transport = Current.UpdateTransport(context.Transport.TransportId, transport =>
        {
            if (updateFn != null)
            {
                transport = updateFn(transport);
            }
            if (messages.Length > 0)
            {
                transport = transport with
                {
                    History = messages.Aggregate(
                        transport.History,
                        (hist, message) => {
                            return hist.AppendItem(HistoryEntry.CreateHistoryEntry(message));
                        }
                    )
                };
            }
            return transport;
        });
        if (transport == null)
        {
            throw new InvalidOperationException("Could not find transport entity in collection.");
        }
        return context with
        {
            Transport = transport
        };
    }
}
