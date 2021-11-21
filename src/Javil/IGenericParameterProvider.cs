namespace Javil;

public interface IGenericParameterProvider
{
    bool HasGenericParameters { get; }
    Collection<GenericParameter> GenericParameters { get; }
}
