using Javil.Attributes;

namespace Javil;

public class TypeDefinition : TypeReference, IMemberDefinition, IAttributeProvider
{
    private Collection<BytecodeAttribute>? attributes;

    public Collection<BytecodeAttribute> Attributes => attributes ??= new Collection<BytecodeAttribute> ();
    public Collection<TypeDefinition> NestedTypes { get; } = new Collection<TypeDefinition> ();
    public Collection<ImplementedInterface> ImplementedInterfaces { get; } = new Collection<ImplementedInterface> ();
    public Collection<FieldDefinition> Fields { get; } = new Collection<FieldDefinition> ();
    public Collection<MethodDefinition> Methods { get; } = new Collection<MethodDefinition> ();
    public string? SourceFileName { get; set; }

    public TypeReference? BaseType { get; }

    internal TypeDefinition (string name, ContainerDefinition container) : base (string.Empty, name, container)
    {
    }

    public TypeDefinition (string @namespace, string name, TypeReference? declaringType, ContainerDefinition container, TypeReference? baseType) : base (@namespace, name, container, declaringType)
    {
        BaseType = baseType;
    }

    public bool IsInterface { get; set; }

    public bool IsStatic { get; set; }

    public bool IsEnum { get; set; }

    public bool IsPublic { get; set; }

    public bool IsProtected { get; set; }

    public bool IsPrivate { get; set; }

    public bool IsAbstract { get; set; }

    public bool IsFinal { get; set; }

    public bool IsNested => DeclaringType is not null;

    public bool IsDeprecated { get; set; }

    public bool HasAttributes => attributes?.Any () == true;
}
