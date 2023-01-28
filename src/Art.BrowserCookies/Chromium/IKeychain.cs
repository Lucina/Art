namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a context-specific keychain accessor.
/// </summary>
public interface IKeychain : IDisposable
{
    /// <summary>
    /// Gets the decrypted content for a buffer.
    /// </summary>
    /// <param name="buffer">Buffer to decrypt.</param>
    /// <returns>Decrypted content.</returns>
    string Unlock(byte[] buffer);
}
