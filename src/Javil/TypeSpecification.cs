namespace Javil
{
    public class TypeSpecification : TypeReference
    {
        public TypeReference ElementType { get; }

        protected TypeSpecification (TypeReference type) : base (string.Empty, string.Empty, type.Container!)
        {
            ElementType = type;
        }

        public override TypeReference? DeclaringType => ElementType.DeclaringType;

        public override string FullName => ElementType.FullName;

        public override string JniName => ElementType.JniName;

        public override string JniFullName => ElementType.JniFullName;

        public override string JniFullNameGenericsErased => ElementType.JniFullNameGenericsErased;

        public override string Name => ElementType.Name;

        public override string Namespace => ElementType.Namespace;

        public override string NestedName => ElementType.NestedName;

        public override string FullNameGenericsErased => ElementType.FullNameGenericsErased;

        public override string GenericName => ElementType.GenericName;

        public override string GetDescriptor (IEnumerable<GenericParameter> genericParameters) => ElementType.GetDescriptor (genericParameters);

        public override string WildcardIndicator {
            get => ElementType.WildcardIndicator;
            set => ElementType.WildcardIndicator = value;
        }

        public override TypeDefinition? Resolve ()
        {
            return ElementType.Resolve ();
        }
    }
}
