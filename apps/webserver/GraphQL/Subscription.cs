public class Subscription
{
    [Subscribe]
    public Group GroupUpdated([EventMessage] Group group)
        => group;

    [Topic("GroupByNameUpdated_{groupName}")]
    [Subscribe]
    public Group GroupByNameUpdated(string groupName, [EventMessage] Group group)
        => group;

    [Subscribe]
    public Driver DriverUpdated([EventMessage] Driver driver)
        => driver;

    [Topic("DriverByIdUpdated_{driverId}")]
    [Subscribe]
    public Driver DriverByIdUpdated(string driverId, [EventMessage] Driver driver)
        => driver;

    [Subscribe]
    public Vehicle VehicleUpdated([EventMessage] Vehicle vehicle)
        => vehicle;

    [Topic("VehicleByIdUpdated_{vehicleId}")]
    [Subscribe]
    public Vehicle VehicleByIdUpdated(string vehicleId, [EventMessage] Vehicle vehicle)
        => vehicle;

    [Subscribe]
    public Transport TransportUpdated([EventMessage] Transport transport)
        => transport;

    [Topic("TransportByIdUpdated_{transportId}")]
    [Subscribe]
    public Transport TransportByIdUpdated(string transportId, [EventMessage] Transport transport)
        => transport;
}
