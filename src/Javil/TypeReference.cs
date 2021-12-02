using Javil.Extensions;

namespace Javil;

public class TypeReference : MemberReference, IGenericParameterProvider
{
    private Collection<GenericParameter>? generic_parameters;

    public virtual string Namespace { get; }
    public virtual string JniName => base.Name;
    public virtual string WildcardIndicator { get; set; } = string.Empty;
    protected virtual string GetGenericJniName => JniName;
    public virtual bool IsArray => false;
    public bool IsPrimitiveType { get; set; }

    public Collection<GenericParameter> GenericParameters => generic_parameters ??= new Collection<GenericParameter> ();
    public bool HasGenericParameters => generic_parameters?.Any () == true;

    protected TypeReference (string @namespace, string name, ContainerDefinition container, TypeReference? declaringType = null) : base (name, declaringType, container)
    {
        Namespace = @namespace;
    }

    public override string FullName {
        get {
            if (IsPrimitiveType)
                return Name;

            if (DeclaringType is not null)
                return $"{DeclaringType.FullName}${GenericName}";

            if (string.IsNullOrWhiteSpace (Namespace))
                return Name;

            return $"{Namespace}.{GenericName}";
        }
    }

    public override string FullNameGenericsErased {
        get {
            if (IsPrimitiveType)
                return Name;

            if (DeclaringType is not null)
                return $"{DeclaringType.FullNameGenericsErased}${Name}";

            if (string.IsNullOrWhiteSpace (Namespace))
                return Name;

            return $"{Namespace}.{Name}";
        }
    }

    public override string Name {
        get {
            if (IsPrimitiveType)
                return TypeExtensions.PrimitiveTypeToName (base.Name);

            return base.Name;
        }
    }

    public static TypeReference CreateFromFullName (string name, ContainerDefinition container)
        => CreateFromSignature (TypeSignature.ParseFullName (name), container);

    public static TypeReference CreateFromSignature (string signature, ContainerDefinition container)
        => CreateFromSignature (TypeSignature.Parse (signature), container);

    public static TypeReference CreateFromSignature (TypeSignature signature, ContainerDefinition container, TypeReference? declaringType = null)
    {
        TypeReference tr;

        if (signature.GenericArguments.Any ()) {
            tr = new GenericInstanceType (signature.Namespace, signature.Name, container, declaringType) { WildcardIndicator = signature.WildcardBounds };

            foreach (var arg in signature.GenericArguments)
                ((GenericInstanceType)tr).GenericArguments.Add (CreateFromSignature (arg, container));

        } else {
            if (signature.IsGenericParameter)
                tr = new GenericParameter (signature.Name, container, declaringType) { WildcardIndicator = signature.WildcardBounds };
            else if (signature.Name == "*")
                tr = new WildcardType (signature.Name, container, declaringType);
            else
                tr = new TypeReference (signature.Namespace, signature.Name, container, declaringType) { WildcardIndicator = signature.WildcardBounds };

            tr.IsPrimitiveType = signature.IsPrimitiveType;
        }

        if (signature.ArrayRank > 0)
            tr = new ArrayType (tr, signature.ArrayRank);

        if (signature.NestedType is not null)
            return CreateFromSignature (signature.NestedType, container, tr);

        return tr;
    }

    protected override IMemberDefinition? ResolveDefinition ()
    {
        return Resolve ();
    }

    public new virtual TypeDefinition? Resolve ()
    {
        if (this is TypeDefinition td)
            return td;

        if (Container is null)
            throw new NotSupportedException ();

        return Container.Resolve (this);
    }

    public virtual string NestedName {
        get {
            if (DeclaringType is null)
                return Name;

            return $"{DeclaringType.NestedName}${Name}";
        }
    }

    public virtual string JniFullName {
        get {
            if (IsPrimitiveType || Name == "*" || Name == "**")
                return JniName;

            //return "L" + FullName.Replace ('.', '/') + ";";
            if (DeclaringType is not null)
                return $"{DeclaringType.JniFullName.TrimEnd (';')}${GetGenericJniName};";

            if (string.IsNullOrWhiteSpace (Namespace))
                return WildcardIndicator + "L" + GetGenericJniName + ";";

            return $"{WildcardIndicator}L{Namespace.Replace ('.', '/')}/{GetGenericJniName};";
        }
    }

    public virtual string JniFullNameGenericsErased {
        get {
            if (IsPrimitiveType)
                return JniName;

            if (DeclaringType is not null)
                return $"{DeclaringType.JniFullNameGenericsErased.TrimEnd (';')}${JniName};";

            if (string.IsNullOrWhiteSpace (Namespace))
                return WildcardIndicator + "L" + JniName + ";";

            return $"{WildcardIndicator}L{Namespace.Replace ('.', '/')}/{JniName};";
        }
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

    public virtual string GetDescriptor (IEnumerable<GenericParameter> genericParameters)
    {
        return JniFullNameGenericsErased;
    }
}
