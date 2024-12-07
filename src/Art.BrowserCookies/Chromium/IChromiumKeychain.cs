namespace Art.BrowserCookies.Chromium;

/// <summary>
/// Represents a context-specific keychain accessor.
/// </summary>
public interface IChromiumKeychain : IDisposable
{
    /// <summary>
    /// Gets the decrypted content for a buffer.
    /// </summary>
    /// <param name="buffer">Buffer to decrypt.</param>
    /// <param name="toolLogHandler">Tool log handler.</param>
    /// <returns>Decrypted content.</returns>
    string Unlock(byte[] buffer, IToolLogHandler? toolLogHandler);
}
