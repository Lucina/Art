using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactDataManager
{
    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public abstract ValueTask<Stream> CreateOutputStreamAsync(string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null);

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="text">Text to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputTextAsync(string text, string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
    {
        using Stream stream = await CreateOutputStreamAsync(file, artifactInfo, inArtifactFolder, path).ConfigureAwait(false);
        using var sw = new StreamWriter(stream);
        await sw.WriteAsync(text).ConfigureAwait(false);
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputJsonAsync<T>(T data, string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
    {
        using Stream stream = await CreateOutputStreamAsync(file, artifactInfo, inArtifactFolder, path).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="inArtifactFolder">If false, place artifact under common root.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    public virtual async ValueTask OutputJsonAsync<T>(T data, JsonSerializerOptions jsonSerializerOptions, string file, ArtifactInfo artifactInfo, bool inArtifactFolder = true, string? path = null)
    {
        using Stream stream = await CreateOutputStreamAsync(file, artifactInfo, inArtifactFolder, path).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions).ConfigureAwait(false);
    }
}
