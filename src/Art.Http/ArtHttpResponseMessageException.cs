using System.Net;
using System.Net.Http.Headers;

namespace Art.Http;

/// <summary>
/// Represents an exception created from an <see cref="HttpResponseMessage"/>.
/// </summary>
public class ArtHttpResponseMessageException : ArtSystemException
{
    /// <summary>
    /// Gets the HTTP status code associated with this exception.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the retry condition associated with this exception.
    /// </summary>
    public RetryConditionHeaderValue? RetryCondition { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtHttpResponseMessageException"/> class.
    /// </summary>
    /// <param name="message">A message describing the current exception.</param>
    public ArtHttpResponseMessageException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtHttpResponseMessageException"/> class.
    /// </summary>
    /// <param name="message">A message describing the current exception.</param>
    /// <param name="httpResponseMessage">Source HTTP response message to source state from.</param>
    public ArtHttpResponseMessageException(string? message, HttpResponseMessage httpResponseMessage) : base(message)
    {
        StatusCode = httpResponseMessage.StatusCode;
        RetryCondition = httpResponseMessage.Headers.RetryAfter;
    }

    /// <summary>
    /// Throws <see cref="ArtHttpResponseMessageException"/> on failure.
    /// </summary>
    /// <param name="httpResponseMessage">HTTP response message.</param>
    /// <exception cref="ArtHttpResponseMessageException">Thrown on HTTP response indicating non-successful response.</exception>
    public static void EnsureSuccessStatusCode(HttpResponseMessage httpResponseMessage)
    {
        try
        {
            httpResponseMessage.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException exception)
        {
            string message = exception.Message;
            throw new ArtHttpResponseMessageException(message, httpResponseMessage);
        }
    }
}
