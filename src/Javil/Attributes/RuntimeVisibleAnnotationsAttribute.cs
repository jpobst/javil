namespace Javil.Attributes;

public class RuntimeVisibleAnnotationsAttribute : BytecodeAttribute
{
    public IList<AnnotationItem> Annotations { get; } = new List<AnnotationItem> ();

    public RuntimeVisibleAnnotationsAttribute (string name) : base (name)
    {
    }
}
