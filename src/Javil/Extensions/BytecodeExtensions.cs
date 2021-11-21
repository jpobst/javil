using Xamarin.Android.Tools.Bytecode;

namespace Javil;

internal static class BytecodeExtensions
{
    public static Nullability GetNullability (this FieldInfo field) => GetNullability (field.Attributes);

    public static Nullability GetParameterNullability (this MethodInfo method, int parameterIndex)
    {
        var annotations = method.Attributes?.OfType<RuntimeInvisibleParameterAnnotationsAttribute> ().FirstOrDefault ()?.Annotations;
        var ann = annotations?.FirstOrDefault (a => a.ParameterIndex == parameterIndex)?.Annotations;

        if (ann?.Any (a => IsNotNullAnnotation (a)) == true)
            return Nullability.NotNull;

        return Nullability.Oblivous;
    }

    public static Nullability GetReturnTypeNullability (this MethodInfo method) => GetNullability (method.Attributes);

    static Nullability GetNullability (AttributeCollection? attributes)
    {
        var annotations = attributes?.OfType<RuntimeInvisibleAnnotationsAttribute> ().FirstOrDefault ()?.Annotations;

        if (annotations?.Any (a => IsNotNullAnnotation (a)) == true)
            return Nullability.NotNull;

        return Nullability.Oblivous;
    }

    static bool IsNotNullAnnotation (Annotation annotation)
    {
        // Android ones plus the list from here:
        // https://stackoverflow.com/questions/4963300/which-notnull-java-annotation-should-i-use
        switch (annotation.Type) {
            case "Landroid/annotation/NonNull;":
            case "Landroidx/annotation/NonNull;":
            case "Landroidx/annotation/RecentlyNonNull;":
            case "Ljavax/validation/constraints/NotNull;":
            case "Ledu/umd/cs/findbugs/annotations/NonNull;":
            case "Ljavax/annotation/Nonnull;":
            case "Lorg/jetbrains/annotations/NotNull;":
            case "Llombok/NonNull;":
            case "Landroid/support/annotation/NonNull;":
            case "Lorg/eclipse/jdt/annotation/NonNull;":
                return true;
        }

        return false;
    }
}
