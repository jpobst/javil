namespace Javil;

public class GenericParameterMapping
{
    private Dictionary<string, string> _mapping = new Dictionary<string, string> ();

    /// <summary>
    /// Add a generic parameter mapping, like "T" -> "java.lang.Object"
    /// </summary>
    public void AddMapping (string name, string value)
    {
        if (name == value)
            return;

        _mapping[name] = value;
    }

    /// <summary>
    /// Adds a generic paramter mapping from a TypeReference.
    /// This is from a GenericInstanceType, or from a TypeReference containing one.
    /// </summary>
    public void AddMappingFromTypeReference (TypeReference typeReference)
    {
        if (typeReference is GenericInstanceType generic_type && typeReference.Resolve () is TypeDefinition resolved_type) {
            for (var i = 0; i < generic_type.GenericArguments.Count; i++)
                AddMapping (resolved_type.GenericParameters[i].Name, generic_type.GenericArguments[i].FullName);
        }
    }

    /// <summary>
    /// Creates a clone of the GenericParameterMapping
    /// </summary>
    public GenericParameterMapping Clone () => new GenericParameterMapping { _mapping = _mapping };

    /// <summary>
    /// Applies the generic parameter mapping to a given TypeReference,
    /// like converting a java.lang.List<T> to java.lang.List<java.lang.Object>
    /// </summary>
    public TypeReference GetMappedReference (TypeReference typeReference)
    {
        if (typeReference is GenericParameter gp) {
            var mapped = GetMapping (gp.Name);

            if (typeReference.Container.FindType (mapped) is TypeDefinition td)
                return td;

            if (mapped != gp.Name)
                return new GenericParameter (mapped, typeReference.Container, typeReference.DeclaringType);

            return gp;
        }

        if (typeReference is GenericInstanceType gt) {
            var mapped = new GenericInstanceType (gt.Namespace, gt.Name, gt.Container, gt.DeclaringType);

            foreach (var ga in gt.GenericArguments)
                mapped.GenericArguments.Add (GetMappedReference (ga));

            return mapped;
        }

        return typeReference;
    }

    private string GetMapping (TypeReference typeReference)
    {
        if (typeReference is GenericParameter gp) {
            var mapping = GetMapping (gp.Name);

            if (mapping != gp.Name)
                return mapping;

            return GetMapping (gp.FullName);
        }

        if (typeReference is GenericInstanceType gt)
            return $"{gt.FullNameGenericsErased}<{string.Join (", ", gt.GenericArguments.Select (ga => GetMapping (ga)))}>";

        return typeReference.FullName;
    }

    private string GetMapping (string name)
    {
        // This has to be recursive for things like:
        // - MyObject extends MyList<Object>
        // - MyList<K> extends MyList<T>
        if (_mapping.TryGetValue (name, out string? value))
            return GetMapping (value);

        return name;
    }
}
