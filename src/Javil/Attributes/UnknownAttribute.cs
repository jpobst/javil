namespace Javil.Attributes;

public class UnknownAttribute : BytecodeAttribute
{
    public byte[] Data { get; set; }

    public UnknownAttribute (string name, byte[] data) : base (name) => Data = data;
}
