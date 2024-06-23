
namespace Art.Tesler;

public class GlobalLocalToolDefaultPropertyProvider : TieredToolDefaultPropertyProvider
{
    public GlobalLocalToolDefaultPropertyProvider(
        IToolDefaultPropertyProvider globalProvider,
        IToolDefaultPropertyProvider localProvider) : base(new IToolDefaultPropertyProvider[] { globalProvider, localProvider })
    {
    }
}