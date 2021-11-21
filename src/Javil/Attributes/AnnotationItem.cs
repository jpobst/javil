namespace Javil.Attributes;

public class AnnotationItem
{
    public string Type { get; set; }

    public IList<KeyValuePair<string, AnnotationElementValue>> Values { get; } = new List<KeyValuePair<string, AnnotationElementValue>> ();

    public AnnotationItem (string type)
    {
        Type = type;
    }
}

public abstract class AnnotationElementValue
{
}

public class AnnotationElementEnum : AnnotationElementValue
{
    public string TypeName { get; set; }
    public string ConstantName { get; set; }

    public AnnotationElementEnum (string typeName, string constantName)
    {
        TypeName = typeName;
        ConstantName = constantName;
    }
}

public class AnnotationElementClassInfo : AnnotationElementValue
{
    public string ClassInfo { get; set; }

    public AnnotationElementClassInfo (string classInfo)
    {
        ClassInfo = classInfo;
    }
}

public class AnnotationElementAnnotation : AnnotationElementValue
{
    public AnnotationItem Annotation { get; set; }

    public AnnotationElementAnnotation (AnnotationItem annotation)
    {
        Annotation = annotation;
    }
}

public class AnnotationElementArray : AnnotationElementValue
{
    public AnnotationElementValue[] Values { get; set; }

    public AnnotationElementArray (AnnotationElementValue[] values)
    {
        Values = values;
    }
}

public class AnnotationElementConstant : AnnotationElementValue
{
    public string Value { get; set; }

    public AnnotationElementConstant (string value)
    {
        Value = value;
    }
}
