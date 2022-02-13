namespace Art.M3U;

/// <summary>
/// Stores encryption information for a stream.
/// </summary>
public class M3UEncryptionInfo
{
    /// <summary>
    /// Encryption method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Key URI.
    /// </summary>
    public string? Uri { get; set; }

    /// <summary>
    /// Key.
    /// </summary>
    public byte[]? Key { get; set; }

    /// <summary>
    /// IV.
    /// </summary>
    public byte[]? Iv { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="M3UEncryptionInfo"/>.
    /// </summary>
    /// <param name="method">Method.</param>
    /// <param name="uri">URI.</param>
    /// <param name="key">Key.</param>
    /// <param name="iv">IV.</param>
    public M3UEncryptionInfo(string method, string? uri = null,byte[]? key = null, byte[]? iv = null)
    {
        Method = method;
        Uri = uri;
        Key = key;
        Iv = iv;
    }
}
