namespace Javil;

public abstract class MemberReference
{
    private IDictionary<string, string>? custom_data;

    public virtual string Name { get; }
    public abstract string FullName { get; }
    public abstract string FullNameGenericsErased { get; }
    public virtual string GenericName => Name;

    public virtual TypeReference? DeclaringType { get; }
    public virtual ContainerDefinition Container { get; }

    public IDictionary<string, string> CustomData => custom_data ??= new Dictionary<string, string> ();

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
