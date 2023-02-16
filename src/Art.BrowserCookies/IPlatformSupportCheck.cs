namespace Art.BrowserCookies;

internal interface IPlatformSupportCheck
{
    static abstract bool IsSupported { get; }
}

internal interface ICookieSourceFactory
{
    static abstract CookieSource? CreateCookieSource(string? profile);
}
