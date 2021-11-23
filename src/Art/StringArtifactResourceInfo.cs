using System.Text;
using System.Text.Json;

namespace Art;

/// <summary>
/// Provides artifact information.
/// </summary>
/// <param name="Resource">Resource.</param>
/// <param name="Key">Artifact key.</param>
/// <param name="File">Filename.</param>
/// <param name="Path">Path.</param>
/// <param name="InArtifactFolder">If false, sent to common directory.</param>
/// <param name="Properties">Resource properties.</param>
public record StringArtifactResourceInfo(string Resource, ArtifactKey Key, string File, string? Path, bool InArtifactFolder, IReadOnlyDictionary<string, JsonElement> Properties) : ArtifactResourceInfo(Key, File, Path, InArtifactFolder, Properties)
{
    /// <summary>
    /// Creates a new instance of <see cref="StringArtifactResourceInfo"/>.
    /// </summary>
    /// <param name="resource">Resource.</param>
    /// <param name="key">Artifact key.</param>
    /// <param name="file">Filename.</param>
    /// <param name="path">Path.</param>
    /// <param name="inArtifactFolder">If false, sent to common directory.</param>
    /// <param name="properties">Resource properties.</param>
    /// <returns>Value.</returns>
    public static StringArtifactResourceInfo Create(string resource, ArtifactKey key, string file, string? path = null, bool inArtifactFolder = true, IReadOnlyDictionary<string, JsonElement>? properties = null)
        => new(resource, key, file, path, inArtifactFolder, properties ?? EmptyProperties);

    /// <inheritdoc/>
    public override bool Exportable => true;

    /// <inheritdoc/>
    public override async ValueTask ExportAsync(Stream stream)
    {
        using var sw = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        await sw.WriteAsync(Resource).ConfigureAwait(false);
    }
}
