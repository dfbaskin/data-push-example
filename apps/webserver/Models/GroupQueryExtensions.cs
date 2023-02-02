
[ExtendObjectType(typeof(Group))]
public class GroupQueryExtensions
{
    private CurrentData Current { get; }

    public GroupQueryExtensions(CurrentData current)
    {
        Current = current ?? throw new ArgumentNullException(nameof(current));
    }

    public IEnumerable<Driver> GetDrivers([Parent] Group group)
    {
        return Current.Drivers.Where(d => d.GroupAssignment == group.Name);
    }
}
