namespace Javil;

public class GenericParameter : TypeReference
{
    public GenericParameter (string name, ContainerDefinition container, TypeReference? declaringType = null) : base (string.Empty, name, container, declaringType)
    {
    }

    public override string Namespace => string.Empty;

    public override string FullName => GetWildcardFullName () + Name;

    public override string NestedName => Name;

    public override string JniFullName => $"{WildcardIndicator}T{Name};";

    public override string JniFullNameGenericsErased => ClassBounds?.JniFullNameGenericsErased ?? InterfaceBounds.FirstOrDefault ()?.JniFullNameGenericsErased ?? "Ljava/lang/Object;";

    public TypeReference? ClassBounds { get; set; }

    public Collection<TypeReference> InterfaceBounds { get; } = new Collection<TypeReference> ();

    public override string GetDescriptor (IEnumerable<GenericParameter> genericParameters)
    {
        if (genericParameters.FirstOrDefault (gp => gp.Name == Name) is GenericParameter gp)
            return gp.JniFullNameGenericsErased;

        return base.GetDescriptor (genericParameters);
    }

    private string GetWildcardFullName ()
    {
        if (WildcardIndicator == "-")
            return "? super ";
        if (WildcardIndicator == "+")
            return "? extends ";

        return string.Empty;
    }

    public override TypeDefinition? Resolve ()
    {
        return null;
    }
}
