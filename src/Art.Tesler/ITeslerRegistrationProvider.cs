using System.CommandLine;
using System.CommandLine.Invocation;

namespace Art.Tesler;

public interface ITeslerRegistrationProvider
{
    void Initialize(Command command);

    IArtifactRegistrationManager CreateArtifactRegistrationManager(InvocationContext context);
}
