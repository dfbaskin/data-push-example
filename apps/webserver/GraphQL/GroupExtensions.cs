
[ExtendObjectType(typeof(Group))]
public class GroupExtensions
{
    private CurrentData Current { get; }

    public GroupExtensions(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    public IEnumerable<Driver> GetDrivers([Parent] Group group)
    {
        return Current.Drivers.Where(d => d.GroupAssignment == group.Name);
    }
}
