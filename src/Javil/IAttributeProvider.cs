using Javil.Attributes;

namespace Javil;

public interface IAttributeProvider
{
    bool HasAttributes { get; }
    Collection<BytecodeAttribute> Attributes { get; }
}
