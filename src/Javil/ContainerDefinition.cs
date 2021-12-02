using Javil.Adapters;
using Javil.Extensions;

namespace Javil;

public sealed class ContainerDefinition
{
    private readonly Dictionary<string, TypeDefinition> types = new Dictionary<string, TypeDefinition> ();

    public string FileName { get; }
    public IReadOnlyCollection<TypeDefinition> Types => types.Values;

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

    public void AddType (TypeDefinition type)
    {
        types.Add (type.FullNameGenericsErased, type);
    }

    public TypeDefinition? FindType (string type)
    {
        // Caller knows what is nested and what is namespace, hooray!
        if (type.Contains ('$')) {
            var t = type.FirstSubset ('$');

            if (types.TryGetValue (t, out var td))
                return FindNestedType (td, type.ChompFirst ('$').Replace ('$', '.'));

            return null;
        }

        // It's all periods, we have to figure out what are namespaces, types, and nested types.
        var curr = type;

        while (curr?.Length > 0) {
            if (types.TryGetValue (curr, out var td)) {

                // Top-level type
                if (curr == type)
                    return td;

                // Nested type
                return FindNestedType (td, type.Substring (curr.Length + 1));
            }

            curr = curr.ChompLast ('.');
        }

        return null;
    }

    private TypeDefinition? FindNestedType (TypeDefinition parent, string type)
    {
        var curr = type.FirstSubset ('.');

        if (parent.NestedTypes.FirstOrDefault (t => t.Name == curr) is TypeDefinition nested) {
            if (!type.Contains ('.'))
                return nested;

            return FindNestedType (nested, type.ChompFirst ('.'));
        }

        return null;
    }
}
