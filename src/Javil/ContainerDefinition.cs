using Javil.Adapters;

namespace Javil;

public sealed class ContainerDefinition
{
    public string FileName { get; }
    public Collection<TypeDefinition> Types { get; } = new Collection<TypeDefinition> ();
    public IContainerResolver Resolver { get; }

    public ContainerDefinition (string fileName, IContainerResolver resolver)
    {
        FileName = fileName;
        Resolver = resolver;
    }

    public static ContainerDefinition ReadContainer (string fileName, ReaderParameters? parameters = null)
        => BytecodeReader.Read (fileName, parameters);

    public TypeDefinition Resolve (TypeReference type)
    {
        return Resolver.Resolve (type);
    }
}
