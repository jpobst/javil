using System.Runtime.CompilerServices;
using Javil.Extensions;

namespace Javil;

public class ImplementedInterface
{
    public TypeReference InterfaceType { get; }
    public TypeDefinition DeclaringType { get; }

    public ImplementedInterface (TypeDefinition declaringType, TypeReference interfaceType)
    {
        DeclaringType = declaringType;
        InterfaceType = interfaceType;
    }
}
