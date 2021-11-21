namespace Javil.Attributes;

public class CodeAttribute : BytecodeAttribute
{
    public byte[] ByteCode { get; set; }
    public ushort MaxStack { get; set; }
    public ushort MaxLocals { get; set; }
    public Collection<CodeExceptionTableEntry> ExceptionTable { get; } = new Collection<CodeExceptionTableEntry> ();

    public CodeAttribute (string name, ushort maxStack, ushort maxLocals, byte[] byteCode) : base (name)
    {
        MaxStack = maxStack;
        MaxLocals = maxLocals;
        ByteCode = byteCode;
    }
}

public sealed class CodeExceptionTableEntry
{
    public ushort StartPC { get; set; }
    public ushort EndPC { get; set; }
    public ushort HandlerPC { get; set; }
    public string CatchType { get; set; }

    public CodeExceptionTableEntry (ushort startPC, ushort endPC, ushort handlerPC, string catchType)
    {
        StartPC = startPC;
        EndPC = endPC;
        HandlerPC = handlerPC;
        CatchType = catchType;
    }
}
