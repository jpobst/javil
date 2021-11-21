namespace Javil.Extensions;

static class TypeExtensions
{
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
}
