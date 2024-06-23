
namespace Art.Tesler;

public class GlobalLocalRunnerDefaultPropertyProvider : TieredRunnerDefaultPropertyProvider
{
    public GlobalLocalRunnerDefaultPropertyProvider(
        IRunnerDefaultPropertyProvider globalProvider,
        IRunnerDefaultPropertyProvider localProvider) : base(new IRunnerDefaultPropertyProvider[] { globalProvider, localProvider })
    {
    }
}