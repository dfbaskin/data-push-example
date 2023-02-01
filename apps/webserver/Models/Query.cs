public class Query
{
    private List<Group> groups { get; } =
        new List<Group>
        {
            new Group(Name: "alpha"),
            new Group(Name: "beta"),
            new Group(Name: "gamma"),
        };

    public IEnumerable<Group> GetGroups() => groups;
}
