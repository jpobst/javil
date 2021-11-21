namespace Javil;

public interface IMemberDefinition
{
    string Name { get; }
    string FullName { get; }

    TypeReference? DeclaringType { get; }
}
