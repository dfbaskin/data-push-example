

public record Group(
  string Name,
  string Description
)
{
    public static IEnumerable<Group> CreateGroups()
    {
        yield return CreateGroup("Oak");
        yield return CreateGroup("Juniper");
        yield return CreateGroup("Pine");
        yield return CreateGroup("Cypress");
        yield return CreateGroup("Cedar");
        yield return CreateGroup("Hickory");
    }

    public static Group CreateGroup(string groupName)
    {
        return new Group(
            Name: groupName,
            Description: Faker.Lorem.Words(7).AsCapitalizedString()
        );
    }
}
