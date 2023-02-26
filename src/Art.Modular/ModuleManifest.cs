using Art.Common;

namespace Art.Modular;

public record ModuleManifest(string BasePath, ModuleManifestContent Content) : IModuleLocation;
