using Javil.Extensions;

namespace Javil;

public class ArrayType : TypeSpecification
{
    public int Rank { get; }

    public ArrayType (TypeReference element, int rank) : base (element)
    {
        Rank = rank;
    }

    public override bool IsArray => true;

    public override string FullName => base.FullName + "[]".Repeat (Rank);

    public override string JniName => ElementType.JniName;

    public override string JniFullName => WildcardIndicator + new string ('[', Rank) + base.JniFullName.TrimStart ('-', '+');

    public override string JniFullNameGenericsErased => WildcardIndicator + new string ('[', Rank) + base.JniFullNameGenericsErased.TrimStart ('-', '+');

    public override string Name => base.Name + "[]".Repeat (Rank);

    public override string Namespace => ElementType.Namespace;

    public override string NestedName => ElementType.NestedName + "[]".Repeat (Rank);

    public override string FullNameGenericsErased => base.FullNameGenericsErased + "[]".Repeat (Rank);

    public override string GenericName => base.GenericName + "[]".Repeat (Rank);

    public override string GetDescriptor (IEnumerable<GenericParameter> genericParameters) => WildcardIndicator + new string ('[', Rank) + base.GetDescriptor (genericParameters);
}
