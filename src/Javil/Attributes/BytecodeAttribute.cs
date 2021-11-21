namespace Javil.Attributes;

public abstract class BytecodeAttribute
{
    public string Name { get; set; }

    protected BytecodeAttribute (string name) => Name = name;
}
