namespace Javil;

public class JavilSettings
{
    private IContainerResolver? resolver;

    public IContainerResolver Resolver {
        get => resolver ??= new BaseContainerResolver ();
        set => resolver = value;
    }
}
