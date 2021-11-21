namespace Javil.Attributes;

public class RuntimeInvisibleParameterAnnotationsAttribute : BytecodeAttribute
{
    public IList<ParameterAnnotation> Annotations { get; } = new List<ParameterAnnotation> ();

    public RuntimeInvisibleParameterAnnotationsAttribute (string name) : base (name)
    {
    }
}

public class ParameterAnnotation
{
    public int Index { get; set; }

    public IList<AnnotationItem> Annotations { get; } = new List<AnnotationItem> ();

    public ParameterAnnotation (int index)
    {
        Index = index;
    }
}
