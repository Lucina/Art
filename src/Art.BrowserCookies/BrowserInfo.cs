namespace Art.BrowserCookies;

internal abstract record BrowserInfo(string Name)
{
    public abstract CookieSource? CreateCookieSource(string? profile);
}

internal record BrowserInfo<T>(string Name) : BrowserInfo(Name) where T : ICookieSourceFactory
{
    public override CookieSource? CreateCookieSource(string? profile) => T.CreateCookieSource(profile);
}
