using Javil.Attributes;

namespace Javil;

public class FieldDefinition : MemberReference, IMemberDefinition, IAttributeProvider
{
    private Collection<BytecodeAttribute>? attributes;

    public TypeReference FieldType { get; }
    public bool IsStatic { get; set; }
    public bool IsPublic { get; set; }
    public bool IsConstant => Value is not null;
    public object? Value { get; set; }
    public bool IsFinal { get; set; }
    public bool IsProtected { get; set; }
    public bool IsSythetic { get; set; }
    public bool IsTransient { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsVolatile { get; set; }
    public bool IsDeprecated { get; set; }
    public Nullability Nullability { get; set; }
    public Collection<BytecodeAttribute> Attributes => attributes ??= new Collection<BytecodeAttribute> ();
    public bool HasAttributes => attributes?.Any () == true;
    public override string FullName => Name;
    public override string FullNameGenericsErased => Name;

    public FieldDefinition (string name, TypeReference fieldType, TypeReference declaringType) : base (name, declaringType, declaringType.Container)
    {
        FieldType = fieldType;
    }

    protected override IMemberDefinition ResolveDefinition ()
    {
        return this;
    }
}
