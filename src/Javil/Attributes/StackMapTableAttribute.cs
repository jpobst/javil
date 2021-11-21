namespace Javil.Attributes;

public class StackMapTableAttribute : BytecodeAttribute
{
    public byte[] Data { get; set; }

    public StackMapTableAttribute (string name, byte[] data) : base (name) => Data = data;
}
