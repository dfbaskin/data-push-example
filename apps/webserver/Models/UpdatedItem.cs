public record UpdatedItem<T>(
    string Id,
    T Original,
    T Updated
) where T : class;
