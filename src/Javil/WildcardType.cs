namespace Javil;

public class WildcardType : TypeReference
{
    public WildcardType (string name, ContainerDefinition container, TypeReference? declaringType = null) : base (string.Empty, "?", container, declaringType)
    {
    }

    public override string JniFullName => "*";
}
