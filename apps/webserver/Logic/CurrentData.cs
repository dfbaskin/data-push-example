using System.Collections.Concurrent;

public class CurrentData
{
    private readonly ConcurrentDictionary<string, Group> groups;
    private readonly ConcurrentDictionary<string, Driver> drivers;
    private readonly ConcurrentDictionary<string, Vehicle> vehicles;
    private readonly ConcurrentDictionary<string, Transport> transports;

    public CurrentData()
    {
        groups = Group
            .CreateGroups()
            .Aggregate(
                new ConcurrentDictionary<string, Group>(),
                (d, v) =>
                {
                    d.TryAdd(v.Name, v);
                    return d;
                }
            );

        drivers = Driver
            .CreateDrivers()
            .Aggregate(
                new ConcurrentDictionary<string, Driver>(),
                (d, v) =>
                {
                    d.TryAdd(v.DriverId, v);
                    return d;
                }
            );

        vehicles = Vehicle
            .CreateVehicles()
            .Aggregate(
                new ConcurrentDictionary<string, Vehicle>(),
                (d, v) =>
                {
                    d.TryAdd(v.VehicleId, v);
                    return d;
                }
            );

        transports = new ConcurrentDictionary<string, Transport>();
    }

    public ICollection<Group> Groups => groups.Values;

    public ICollection<Driver> Drivers => drivers.Values;

    public ICollection<Vehicle> Vehicles => vehicles.Values;

    public ICollection<Transport> Transports => transports.Values;

    public Driver? UpdateDriver(string driverId, Func<Driver, Driver> updateFn)
    {
        return UpdateItem(driverId, drivers, updateFn);
    }

    public Vehicle? UpdateVehicle(string vehicleId, Func<Vehicle, Vehicle> updateFn)
    {
        return UpdateItem(vehicleId, vehicles, updateFn);
    }

    public bool AddTransport(Transport transport)
    {
        return transports.TryAdd(transport.TransportId, transport);
    }

    public Transport? UpdateTransport(string transportId, Func<Transport, Transport> updateFn)
    {
        return UpdateItem(transportId, transports, updateFn);
    }

    private T? UpdateItem<T>(string itemId, ConcurrentDictionary<string, T> items, Func<T, T> updateFn)
        where T : class
    {
        if (!items.TryGetValue(itemId, out T? current))
        {
            return null;
        }

        var updated = updateFn(current);
        if (!items.TryUpdate(itemId, updated, current))
        {
            return null;
        }

        return updated;
    }
}
