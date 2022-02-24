using Art.Resources;

namespace Art;

/// <summary>
/// Represents an <see cref="Resources.ArtifactResourceInfo"/> with an associated <see cref="Art.ItemStateFlags"/>.
/// </summary>
/// <param name="ArtifactResourceInfo">Artifact resource information.</param>
/// <param name="State">Item state flags.</param>
public readonly record struct ArtifactResourceInfoWithState(ArtifactResourceInfo ArtifactResourceInfo, ItemStateFlags State);
