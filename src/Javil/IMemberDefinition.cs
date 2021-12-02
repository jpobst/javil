namespace Javil;

public interface IMemberDefinition : ICustomDataProvider
{
    string Name { get; }
    string FullName { get; }

    TypeReference? DeclaringType { get; }
}
