using Javil.Extensions;
using Xamarin.Android.Tools.Bytecode;

namespace Javil;

public class CheckedException
{
    public MethodDefinition Method { get; }
    public TypeReference Type { get; }

    public CheckedException (MethodDefinition method, TypeInfo type)
    {
        Method = method;

        if (type.TypeSignature.HasValue ())
            Type = TypeReference.CreateFromSignature (type.TypeSignature, method.Container);
        else
            Type = TypeReference.CreateFromFullName (type.BinaryName, method.Container);
    }
}
