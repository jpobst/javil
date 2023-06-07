using System.Security.Cryptography.X509Certificates;

namespace Javil;

public class GenericParameterMapping
{
    private Dictionary<string, string> _mapping = new Dictionary<string, string> ();

    public void AddMapping (string name, string value)
    {
        if (name == value)
            return;

        _mapping[name] = value;
    }

    public void AddMappingFromTypeReference (TypeReference typeReference)
    {
        if (typeReference is GenericInstanceType generic_type && typeReference.Resolve () is TypeDefinition resolved_type) {
            for (var i = 0; i < generic_type.GenericArguments.Count; i++)
                AddMapping (resolved_type.GenericParameters[i].Name, generic_type.GenericArguments[i].FullName);
        }
    }

    public string GetMapping (TypeReference typeReference)
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
