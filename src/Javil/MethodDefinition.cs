using System.Text;

namespace Javil;

public class MethodDefinition : MemberReference, IMemberDefinition, IGenericParameterProvider
{
    private Collection<CheckedException>? checkedExceptions;
    private Collection<GenericParameter>? generic_parameters;
    private Collection<ParameterDefinition>? parameters;

    public override string FullName => Name;
    public override string FullNameGenericsErased => Name;
    public TypeReference ReturnType { get; set; }
    public bool IsPublic { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsProtected { get; set; }
    public bool IsStatic { get; set; }
    public bool IsFinal { get; set; }
    public bool IsSynchronized { get; set; }
    public bool IsBridge { get; set; }
    public bool IsVarargs { get; set; }
    public bool IsNative { get; set; }
    public bool IsAbstract { get; set; }
    public bool IsStrict { get; set; }
    public bool IsSynthetic { get; set; }
    public bool IsConstructor => Name == "<init>";
    public bool IsDeprecated { get; set; }
    public Nullability ReturnTypeNullability { get; set; }
    public bool HasCheckedExceptions => checkedExceptions?.Any () == true;
    public bool HasGenericParameters => generic_parameters?.Any () == true;
    public bool HasParameters => parameters?.Any () == true;
    public Collection<CheckedException> CheckedExceptions => checkedExceptions ??= new Collection<CheckedException> ();
    public Collection<GenericParameter> GenericParameters => generic_parameters ??= new Collection<GenericParameter> ();
    public Collection<ParameterDefinition> Parameters => parameters ??= new Collection<ParameterDefinition> ();

    public MethodDefinition (string name, TypeReference returnType, TypeReference declaringType) : base (name, declaringType, declaringType.Container)
    {
        ReturnType = returnType;
    }

    public MethodDefinition? FindDeclaredBaseMethodOrDefault ()
    {
        var candidate = FindBaseMethodOrDefault ();

        if (candidate is null)
            return null;

        while (true) {
            var new_candidate = candidate.FindBaseMethodOrDefault ();

            if (new_candidate is null)
                return candidate;

            candidate = new_candidate;
        }
    }

    public MethodDefinition? FindBaseMethodOrDefault ()
    {
        // Static methods cannot be overridden
        if (IsStatic) 
            return null;

        if (DeclaringType?.Resolve ()?.BaseType is TypeReference base_type) {
            var mapping = new GenericParameterMapping ();
            mapping.AddMappingFromTypeReference (base_type);

            return FindBaseMethod (base_type.Resolve (), mapping);
        }

        return null;
    }

    public virtual IEnumerable<GenericParameter> GetGenericParametersInScope ()
    {
        if (HasGenericParameters)
            foreach (var gp in GenericParameters)
                yield return gp;

        if (DeclaringType is not null)
            foreach (var gp in DeclaringType.GetGenericParametersInScope ())
                yield return gp;
    }

    private MethodDefinition? FindBaseMethod (TypeDefinition? type, GenericParameterMapping mapping)
    {
        if (type is null)
            return null;

        var candidates = type.Methods.OfType<MethodDefinition> ().Where (m => m.Name == Name && m.Parameters.Count == Parameters.Count);

        foreach (var candidate in candidates)
            if (AreMethodsCompatible (this, candidate, mapping))
                return candidate;

        if (type.BaseType is TypeReference base_type) {
            mapping.AddMappingFromTypeReference (base_type);
            return FindBaseMethod (base_type.Resolve (), mapping);
        }

        return null;
    }

    protected override IMemberDefinition? ResolveDefinition ()
    {
        return this;
    }

    public override string GenericName {
        get {
            if (!HasGenericParameters)
                return Name;

            var sb = new StringBuilder ();

            sb.Append (Name);
            sb.Append ('<');
            sb.Append (string.Join (", ", GenericParameters.Select (a => a.FullName)));
            sb.Append ('>');

            return sb.ToString ();
        }
    }

    private bool AreMethodsCompatible (MethodDefinition method, MethodDefinition candidate, GenericParameterMapping mapping)
    {
        for (var i = 0; i < method.Parameters.Count; i++) {
            var p1 = method.Parameters[i].ParameterType.FullName;
            var p2 = mapping.GetMapping (candidate.Parameters[i].ParameterType);

            if (p1 == p2)
                continue;

            // TODO: Need a more elegant/correct way of handling the following cases
            if (p1.Replace ("**", "java.lang.Object, java.lang.Object") == p2)
                continue;

            if (p1.Replace ("?", "java.lang.Object") == p2)
                continue;

            if (p1.Replace ("java.lang.Class<?>", "java.lang.Class") == p2.Replace ("java.lang.Class<?>", "java.lang.Class"))
                continue;

            return false;
        }

        return true;
    }

    // (Landroid/content/ComponentName;Ljava/lang/String;Ljava/util/List;)Z
    public string GetDescriptor ()
    {
        var sb = new StringBuilder ();
        var gps = GetGenericParametersInScope ().ToArray ();

        sb.Append ('(');

        // Add this extra parameter for constructors in non-static nested types
        if (IsConstructor && DeclaringType is TypeDefinition td && !td.IsStatic && td.NestedName.Contains ('$') == true)
            sb.Append (td.DeclaringType?.JniFullName);

        foreach (var p in Parameters)
            sb.Append (p.ParameterType.GetDescriptor (gps));

        sb.Append (')');

        sb.Append (ReturnType.GetDescriptor (gps));

        return sb.ToString ();
    }

    public override string ToString ()
    {
        return $"{ReturnType.Name} {FullName} ({string.Join (", ", Parameters.Select (p => $"{p.ParameterType.Name} {p.Name}"))})";
    }
}
