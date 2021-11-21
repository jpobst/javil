namespace Javil;

public abstract class MemberReference
{
    public virtual string Name { get; }
    public abstract string FullName { get; }
    public abstract string FullNameGenericsErased { get; }
    public virtual string GenericName => Name;

    public virtual TypeReference? DeclaringType { get; }
    public virtual ContainerDefinition Container { get; }

    protected MemberReference (string name, TypeReference? declaringType, ContainerDefinition container)
    {
        Name = name;
        DeclaringType = declaringType;
        Container = container;
    }

    public override string ToString () => FullName;

    public IMemberDefinition? Resolve ()
    {
        return ResolveDefinition ();
    }

    protected abstract IMemberDefinition? ResolveDefinition ();
}
