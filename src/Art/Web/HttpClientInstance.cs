namespace Art.Web;

/// <summary>
/// <see cref="System.Net.Http.HttpClient"/> instance with associated <see cref="System.Net.Http.HttpClientHandler"/>.
/// </summary>
/// <param name="HttpClient">Client.</param>
/// <param name="HttpClientHandler">Client handler.</param>
public record HttpClientInstance(HttpClient HttpClient, HttpClientHandler HttpClientHandler);