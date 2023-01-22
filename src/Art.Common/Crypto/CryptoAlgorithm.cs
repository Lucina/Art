namespace Art.Common.Crypto;

/// <summary>
/// Represents crypto algorithm.
/// </summary>
public enum CryptoAlgorithm
{
    /// <summary>
    /// <see cref="System.Security.Cryptography.Aes"/>.
    /// </summary>
    Aes,
    /// <summary>
    /// Blowfish.
    /// </summary>
    Blowfish,
    /// <summary>
    /// <see cref="System.Security.Cryptography.DES"/>.
    /// </summary>
    DES,
    /// <summary>
    /// <see cref="System.Security.Cryptography.TripleDES"/>.
    /// </summary>
    TripleDES,
    /// <summary>
    /// <see cref="System.Security.Cryptography.RC2"/>.
    /// </summary>
    RC2,
    /// <summary>
    /// XOR.
    /// </summary>
    Xor
}
