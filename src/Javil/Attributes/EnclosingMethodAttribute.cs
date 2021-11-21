namespace Javil.Attributes;

public class EnclosingMethodAttribute : BytecodeAttribute
{
    public string Class { get; set; }
    public ConstantNameAndTypeItem? Method { get; set; }

    public EnclosingMethodAttribute (string name, string @class, ConstantNameAndTypeItem? method) : base (name)
    {
        Class = @class;
        Method = method;
    }
}
