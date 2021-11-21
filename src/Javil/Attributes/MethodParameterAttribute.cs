namespace Javil.Attributes;

public class MethodParameterAttribute : BytecodeAttribute
{
    public Collection<MethodParameterInfo> Parameters { get; } = new Collection<MethodParameterInfo> ();

    public MethodParameterAttribute (string name) : base (name)
    {
    }
}

public sealed class MethodParameterInfo
{
    public string? Name { get; set; }
    public MethodParameterAccessFlags AccessFlags { get; set; }

    public MethodParameterInfo (string? name, MethodParameterAccessFlags accessFlags)
    {
        Name = name;
        AccessFlags = accessFlags;
    }
}

[Flags]
public enum MethodParameterAccessFlags
{
    None,
    Final = 0x0010,
    Synthetic = 0x1000,
    Mandated = 0x8000,
}
