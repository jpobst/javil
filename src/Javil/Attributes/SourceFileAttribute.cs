namespace Javil.Attributes;

public class SourceFileAttribute : BytecodeAttribute
{
    public string FileName { get; set; }

    public SourceFileAttribute (string name, string filename) : base (name)
    {
        FileName = filename;
    }
}
