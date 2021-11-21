namespace Javil.Attributes;

public class LocalVariableTableAttribute : BytecodeAttribute
{
    public Collection<LocalVariableTableEntry> LocalVariables { get; } = new Collection<LocalVariableTableEntry> ();

    public LocalVariableTableAttribute (string name) : base (name)
    {
    }
}

public sealed class LocalVariableTableEntry
{
    public ushort StartPC { get; set; }
    public ushort Length { get; set; }
    public ushort Index { get; set; }
    public string Name { get; set; }
    public string Descriptor { get; set; }

    public LocalVariableTableEntry (ushort startPC, ushort length, ushort index, string name, string descriptor)
    {
        StartPC = startPC;
        Length = length;
        Index = index;
        Name = name;
        Descriptor = descriptor;
    }
}
