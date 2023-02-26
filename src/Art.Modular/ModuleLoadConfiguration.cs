using System.Collections.Immutable;

namespace Art.Modular;

public record ModuleLoadConfiguration(ImmutableHashSet<string> PassthroughAssemblies);
