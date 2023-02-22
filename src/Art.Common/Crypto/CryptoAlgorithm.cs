namespace Art.Common.Crypto;

/// <summary>
/// Represents crypto algorithm identity.
/// </summary>
public enum CryptoAlgorithm
{
    /// <summary>
    /// AES block cipher family.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.Aes"/>
    Aes,
    /// <summary>
    /// Blowfish block cipher.
    /// </summary>
    Blowfish,
    /// <summary>
    /// DES block cipher.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.DES"/>
    DES,
    /// <summary>
    /// Triple DES block cipher.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.TripleDES"/>
    TripleDES,
    /// <summary>
    /// RC2 block cipher.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.RC2"/>
    RC2,
    /// <summary>
    /// XOR key cipher.
    /// </summary>
    Xor
}
