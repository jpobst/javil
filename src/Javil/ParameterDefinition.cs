namespace Javil;

public class ParameterDefinition : ICustomDataProvider
{
    private IDictionary<string, string>? custom_data;

    public string Name { get; set; }
    public int Index { get; set; }
    public TypeReference ParameterType { get; set; }
    public Nullability Nullability { get; set; }
    public MethodDefinition Method { get; set; }

    public IDictionary<string, string> CustomData => custom_data ??= new Dictionary<string, string> ();

    public ParameterDefinition (MethodDefinition method, string name, TypeReference type, int index, Nullability nullability)
    {
        Method = method;
        Name = name;
        ParameterType = type;
        Index = index;
        Nullability = nullability;
    }
}
