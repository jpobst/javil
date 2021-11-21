using System.Text;
using Javil.Internal;

namespace Javil;

public class TypeSignature
{
    public string Namespace { get; set; }
    public string Name { get; set; }
    public int ArrayRank { get; set; }
    public bool IsPrimitiveType { get; set; }
    public bool IsGenericParameter { get; set; }
    public string WildcardBounds { get; set; } = string.Empty;
    public string WildcardIndicator { get; set; } = string.Empty;
    public TypeSignature? NestedType { get; set; }

    private Collection<TypeSignature>? generic_arguments;
    public Collection<TypeSignature> GenericArguments => generic_arguments ??= new Collection<TypeSignature> ();
    public bool HasGenericArguments => generic_arguments?.Any () == true;

    internal TypeSignature (string ns, string name)
    {
        Namespace = ns;
        Name = name;
    }

    public static TypeSignature Parse (string s) => SignatureParser.Parse (s);

    public static TypeSignature ParseFullName (string s) => SignatureParser.Parse ("L" + s + ";");

    public TypeSignature GetMostNestedType ()
    {
        if (NestedType == null)
            return this;

        return NestedType.GetMostNestedType ();
    }

    public override string ToString ()
    {
        var sb = new StringBuilder ();

        var most_nested_type = this;

        while (most_nested_type.NestedType is not null)
            most_nested_type = most_nested_type.NestedType;

        sb.Append (WildcardBounds);

        sb.Append (new string ('[', most_nested_type.ArrayRank));

        if (IsPrimitiveType || Name == "*" || Name == "**") {
            sb.Append (Name);
            return sb.ToString ();
        }

        sb.Append (WildcardIndicator);

        sb.Append (IsGenericParameter ? 'T' : 'L');

        if (!string.IsNullOrWhiteSpace (Namespace))
            sb.Append (Namespace.Replace ('.', '/') + "/");

        sb.Append (Name);

        if (HasGenericArguments) {
            sb.Append ('<');

            sb.Append (string.Concat (GenericArguments.Select (a => a.ToString ())));

            sb.Append ('>');

        }

        if (NestedType is not null) {
            sb.Append ('$');
            sb.Append (NestedType.ToString ().TrimStart ('[')[1..^1]);
        }

        sb.Append (';');

        return sb.ToString ();
    }
}
