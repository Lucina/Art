/*
 * https://github.com/Riboe/M3USharp/blob/master/M3UParser/M3UFile.cs
 * @9d5d04d
 * MIT-licensed by Riboe
 */

namespace Art.M3U;

/// <summary>
/// Represents an M3U container.
/// </summary>
public class M3UFile
{
    /// <summary>
    /// Encryption information.
    /// </summary>
    public M3UEncryptionInfo? EncryptionInfo { get; set; }

    private readonly List<StreamInfo> _streams = new();

    /// <summary>
    /// Data lines.
    /// </summary>
    public List<string> DataLines { get; set; } = new();

    /// <summary>
    /// M3U version.
    /// </summary>
    public string? Version { get; internal set; }

    /// <summary>
    /// Available streams.
    /// </summary>
    public IReadOnlyList<StreamInfo> Streams => _streams;

    internal void AddStream(StreamInfo stream)
    {
        _streams.Add(stream);
    }
}
