public sealed partial class SimulationWorker
{
    private TransportContext UpdateDriver(
        TransportContext context,
        string message
    ) => UpdateDriver(context, null, message);

    private TransportContext UpdateDriver(
        TransportContext context,
        Func<Driver, Driver>? updateFn,
        string? message = null
    )
    {
        var driver = Current.UpdateDriver(context.Driver.DriverId, driver =>
        {
            if (updateFn != null)
            {
                driver = updateFn(driver);
            }
            if (message != null)
            {
                driver = driver with
                {
                    History = driver.History.AppendItem(HistoryEntry.CreateHistoryEntry(message))
                };
            }
            return driver;
        });
        if (driver == null)
        {
            throw new InvalidOperationException("Could not driver entity in collection.");
        }
        return context with
        {
            Driver = driver
        };
    }

    private TransportContext UpdateVehicle(
        TransportContext context,
        string message
    ) => UpdateVehicle(context, null, message);

    private TransportContext UpdateVehicle(
        TransportContext context,
        Func<Vehicle, Vehicle>? updateFn,
        string? message = null
    )
    {
        var vehicle = Current.UpdateVehicle(context.Vehicle.VehicleId, vehicle =>
        {
            if (updateFn != null)
            {
                vehicle = updateFn(vehicle);
            }
            if (message != null)
            {
                vehicle = vehicle with
                {
                    History = vehicle.History.AppendItem(HistoryEntry.CreateHistoryEntry(message))
                };
            }
            return vehicle;
        });
        if (vehicle == null)
        {
            throw new InvalidOperationException("Could not vehicle entity in collection.");
        }
        return context with
        {
            Vehicle = vehicle
        };
    }

    private TransportContext UpdateTransport(
        TransportContext context,
        string message
    ) => UpdateTransport(context, null, message);

    private TransportContext UpdateTransport(
        TransportContext context,
        Func<Transport, Transport>? updateFn,
        string? message = null
    )
    {
        var transport = Current.UpdateTransport(context.Transport.TransportId, transport =>
        {
            if (updateFn != null)
            {
                transport = updateFn(transport);
            }
            if (message != null)
            {
                transport = transport with
                {
                    History = transport.History.AppendItem(HistoryEntry.CreateHistoryEntry(message))
                };
            }
            return transport;
        });
        if (transport == null)
        {
            throw new InvalidOperationException("Could not transport entity in collection.");
        }
        return context with
        {
            Transport = transport
        };
    }
}
