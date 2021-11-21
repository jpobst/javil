namespace Javil.Attributes;

public class InnerClassesAttribute : BytecodeAttribute
{
    public Collection<InnerClassInfo> InnerClasses { get; } = new Collection<InnerClassInfo> ();

    public InnerClassesAttribute (string name) : base (name)
    {
    }
}

public sealed class InnerClassInfo
{
    public string? InnerName { get; set; }
    public string? OuterClassName { get; set; }

    public InnerClassInfo (string? innerName, string? outerClassName)
    {
        InnerName = innerName;
        OuterClassName = outerClassName;
    }
}
