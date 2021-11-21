namespace Javil.Attributes;

public class SignatureAttribute : BytecodeAttribute
{
    public string Signature { get; set; }

    public SignatureAttribute (string name, string signature) : base (name)
    {
        Signature = signature;
    }
}
