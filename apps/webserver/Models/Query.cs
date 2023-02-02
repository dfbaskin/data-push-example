public class Query
{
    private List<Group> groups = Group.CreateGroups().ToList();
    private List<Driver> drivers = Driver.CreateDrivers().ToList();

    public IEnumerable<Group> GetGroups() => groups;
    public IEnumerable<Driver> GetDrivers() => drivers;
}
