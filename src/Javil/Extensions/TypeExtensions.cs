namespace Javil.Extensions;

static class TypeExtensions
{
    static readonly GenericParameterMapping no_mapping = new GenericParameterMapping ();

    public static string PrimitiveTypeToName (string type)
    {
        return type switch {
            "B" => "byte",
            "C" => "char",
            "D" => "double",
            "F" => "float",
            "I" => "int",
            "J" => "long",
            "S" => "short",
            "V" => "void",
            "Z" => "boolean",
            _ => throw new NotImplementedException (),
        };
    }

    public static bool AreMethodsCompatible (MethodDefinition method, MethodDefinition candidate, GenericParameterMapping mapping)
    {
        for (var i = 0; i < method.Parameters.Count; i++) {
            if (!AreParametersCompatible (method.Parameters[i].ParameterType, candidate.Parameters[i].ParameterType, mapping))
                return false;
        }

        return true;
    }

    public static bool AreParametersCompatible (TypeReference type1, TypeReference type2, GenericParameterMapping? mapping = null)
    {
        mapping ??= no_mapping;

        if (type1.ToString () == mapping.GetMappedReference (type2).ToString ())
            return true;

        // TODO: Is this ok?
        // It catches cases like:
        // - java.lang.Class[] = java.lang.Class<?>[]
        // - java.util.Map<**> = java.util.Map<java.lang.Object, java.lang.Object>
        if (type1.JniFullNameGenericsErased == type2.JniFullNameGenericsErased)
            return true;

        return false;
    }

    public static bool ImplementsInterface (this TypeDefinition type, TypeDefinition iface)
    {
        if (!iface.IsInterface)
            throw new ArgumentException ($"'{iface.FullName}' is not an interface.");

        foreach (var i in type.ImplementedInterfaces) {
            if (i.InterfaceType.FullNameGenericsErased == iface.FullNameGenericsErased)
                return true;

            // Recurse through interfaces
            if (i.InterfaceType.Resolve () is TypeDefinition td && td.ImplementsInterface (iface))
                return true;
        }

        // TODO: Should this check base types?

        return false;
    }
}
