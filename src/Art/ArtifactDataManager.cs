using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Art;

/// <summary>
/// Represents a manager for artifact data.
/// </summary>
public abstract class ArtifactDataManager
{
    /// <summary>
    /// Attempts to get info for the artifact with the specified ID.
    /// </summary>
    /// <param name="id">Artifact ID.</param>
    /// <returns>Task returning retrieved artifact, if it exists.</returns>
    public abstract Task<ArtifactInfo?> TryGetInfoAsync(string id);

    /// <summary>
    /// Creates an output stream for a file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task returning a writeable stream to write an output to.</returns>
    public abstract Task<Stream> CreateOutputStreamAsync(ArtifactInfo artifactInfo, string file, string? path = null);

    /// <summary>
    /// Registers artifact as known.
    /// </summary>
    /// <param name="artifactInfo">Artifact to register.</param>
    /// <returns>Task.</returns>
    public abstract Task AddInfoAsync(ArtifactInfo artifactInfo);

    /// <summary>
    /// Tests if artifact is recognizably new.
    /// </summary>
    /// <param name="artifactInfo">Artifact to check.</param>
    /// <returns>True if this is a new artifact (newer than whatever exists with the same ID).</returns>
    public async Task<bool> IsNewArtifactAsync(ArtifactInfo artifactInfo)
    {
        return (await TryGetInfoAsync(artifactInfo.Id).ConfigureAwait(false)) is not { } oldArtifact || oldArtifact.UpdateDate == null || artifactInfo.UpdateDate != null && oldArtifact.UpdateDate.Value < artifactInfo.UpdateDate.Value;
    }

    /// <summary>
    /// Outputs a text file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="text">Text to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    public virtual async Task OutputTextAsync(ArtifactInfo artifactInfo, string text, string file, string? path = null)
    {
        using Stream stream = await CreateOutputStreamAsync(artifactInfo, file, path).ConfigureAwait(false);
        using var sw = new StreamWriter(stream);
        await sw.WriteAsync(text).ConfigureAwait(false);
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="data">Data to output.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    public virtual async Task OutputJsonAsync<T>(ArtifactInfo artifactInfo, T data, string file, string? path = null)
    {
        using Stream stream = await CreateOutputStreamAsync(artifactInfo, file, path).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
    }

    /// <summary>
    /// Outputs a JSON-serialized file for the specified artifact.
    /// </summary>
    /// <param name="artifactInfo">Artifact target.</param>
    /// <param name="data">Data to output.</param>
    /// <param name="jsonSerializerOptions">Serialization options.</param>
    /// <param name="file">Target filename.</param>
    /// <param name="path">File path to prepend.</param>
    /// <returns>Task.</returns>
    public virtual async Task OutputJsonAsync<T>(ArtifactInfo artifactInfo, T data, JsonSerializerOptions jsonSerializerOptions, string file, string? path = null)
    {
        using Stream stream = await CreateOutputStreamAsync(artifactInfo, file, path).ConfigureAwait(false);
        await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions).ConfigureAwait(false);
    }
}
