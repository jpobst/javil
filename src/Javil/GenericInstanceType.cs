using System.Text;

namespace Javil;

public class GenericInstanceType : TypeReference
{
    public Collection<TypeReference> GenericArguments { get; } = new Collection<TypeReference> ();

    public GenericInstanceType (string @namespace, string name, ContainerDefinition container, TypeReference? declaringType = null) : base (@namespace, name, container, declaringType)
    {
    }

    public override string GenericName {
        get {
            var sb = new StringBuilder ();

            sb.Append (Name);
            sb.Append ('<');
            sb.Append (string.Join (", ", GenericArguments.Select (a => a.FullName)));
            sb.Append ('>');

            return sb.ToString ();
        }
    }

    protected override string GetGenericJniName {
        get {
            var sb = new StringBuilder ();

            sb.Append (Name);
            sb.Append ('<');
            sb.Append (string.Concat (GenericArguments.Select (a => a.JniFullName)));
            sb.Append ('>');

            return sb.ToString ();
        }
    }
}
