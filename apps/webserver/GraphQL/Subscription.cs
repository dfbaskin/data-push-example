public class Subscription
{
    [Subscribe]
    public Group GroupUpdated([EventMessage] Group group) => group;

    [Subscribe]
    public Driver DriverUpdated([EventMessage] Driver driver) => driver;

    [Subscribe]
    public Vehicle VehicleUpdated([EventMessage] Vehicle vehicle) => vehicle;

    [Subscribe]
    public Transport TransportUpdated([EventMessage] Transport transport) => transport;
}
