namespace Javil.Attributes;

public class RuntimeInvisibleAnnotationsAttribute : BytecodeAttribute
{
    public IList<AnnotationItem> Annotations { get; } = new List<AnnotationItem> ();

    public RuntimeInvisibleAnnotationsAttribute (string name) : base (name)
    {
    }
}
