namespace Javil;

public class BaseContainerResolver : IContainerResolver
{
    private readonly Collection<ContainerDefinition> containers = new Collection<ContainerDefinition> ();
    private readonly ContainerDefinition primitive_container;
    private readonly TypeReference[] primitive_types;

    public BaseContainerResolver ()
    {
        primitive_container = new ContainerDefinition ("built-in-types.jar", this);

        primitive_types = new[] {
                new TypeDefinition ("byte", primitive_container),
                new TypeDefinition ("char", primitive_container),
                new TypeDefinition ("double", primitive_container),
                new TypeDefinition ("float", primitive_container),
                new TypeDefinition ("int", primitive_container),
                new TypeDefinition ("long", primitive_container),
                new TypeDefinition ("short", primitive_container),
                new TypeDefinition ("void", primitive_container),
                new TypeDefinition ("boolean", primitive_container)
            };
    }

    public void AddContainer (ContainerDefinition container)
    {
        containers.Add (container);
    }

    public void AddContainer (string filename)
    {
        containers.Add (new ContainerDefinition (filename, this));
    }

    public TypeDefinition Resolve (MemberReference member)
    {
        // Check for primitive type
        if (primitive_types.FirstOrDefault (t => t.FullName == member.FullName) is TypeDefinition pt)
            return pt;

        // First check in the same container as the caller
        if (member.Container is not null && Resolve (member.Container, member) is TypeDefinition td)
            return td;

        // Check the rest of the containers
        foreach (var container in containers.Except (new[] { member.Container }))
            if (container is not null && Resolve (container, member) is TypeDefinition td2)
                return td2;

        throw new ResolutionException (member);
    }

    private TypeDefinition? Resolve (ContainerDefinition container, MemberReference member)
    {
        var full_name = member is TypeSpecification ts ? ts.ElementType.FullNameGenericsErased : member.FullNameGenericsErased;

        return container.FindType (full_name);
    }
}
