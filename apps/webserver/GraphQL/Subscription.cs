public class Subscription
{
    [Subscribe]
    public Group GroupUpdated([EventMessage] Group group) => group;
}
