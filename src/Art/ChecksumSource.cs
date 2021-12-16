using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Art;

/// <summary>
/// Identifies a checksum source.
/// </summary>
public class ChecksumSource
{
    /// <summary>
    /// Main instance of a SHA256 checksum source.
    /// </summary>
    public static readonly ChecksumSource SHA1 = new("SHA1", System.Security.Cryptography.SHA1.Create);

    /// <summary>
    /// Main instance of a SHA256 checksum source.
    /// </summary>
    public static ChecksumSource SHA256 = new("SHA256", System.Security.Cryptography.SHA256.Create);

    /// <summary>
    /// Main instance of a SHA256 checksum source.
    /// </summary>
    public static readonly ChecksumSource SHA384 = new("SHA384", System.Security.Cryptography.SHA384.Create);

    /// <summary>
    /// Main instance of a SHA256 checksum source.
    /// </summary>
    public static readonly ChecksumSource SHA512 = new("SHA512", System.Security.Cryptography.SHA512.Create);

    /// <summary>
    /// Main instance of a SHA256 checksum source.
    /// </summary>
    public static readonly ChecksumSource MD5 = new("MD5", System.Security.Cryptography.MD5.Create);

    /// <summary>
    /// Default checksum sources.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, ChecksumSource> DefaultSources;

    static ChecksumSource()
    {
        DefaultSources = new[] { SHA1, SHA256, SHA384, SHA512, MD5 }.ToDictionary(source => source.Id);
    }

    /// <summary>Algorithm ID.</summary>
    public string Id { get; }

    /// <summary>Hash algorithm function, if available.</summary>
    public Func<HashAlgorithm>? HashAlgorithmFunc { get; }

    /// <summary>
    /// Identifies a checksum source.
    /// </summary>
    /// <param name="id">Algorithm ID.</param>
    /// <param name="hashAlgorithmFunc">Hash algorithm function, if available.</param>
    public ChecksumSource(string id, Func<HashAlgorithm>? hashAlgorithmFunc)
    {
        Id = id;
        HashAlgorithmFunc = hashAlgorithmFunc;
    }

    /// <summary>
    /// Attempts to get a hash algorithm for the specified source id.
    /// </summary>
    /// <param name="id">Algorithm id (e.g. from <see cref="SHA256"/>).</param>
    /// <param name="hashAlgorithm">Hash algorithm, if available.</param>
    /// <returns>True if hash algorithm found.</returns>
    public static bool TryGetHashAlgorithm(string? id, [NotNullWhen(true)] out HashAlgorithm? hashAlgorithm)
    {
        if (id == null || !DefaultSources.TryGetValue(id, out ChecksumSource? source) || source.HashAlgorithmFunc is not { } func)
        {
            hashAlgorithm = null;
            return false;
        }
        try
        {
            hashAlgorithm = func.Invoke();
            return true;
        }
        catch
        {
            hashAlgorithm = null;
            return false;
        }
    }
}
