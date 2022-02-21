using System.Net;
using System.Net.Http.Headers;

namespace Art;

/// <summary>
/// Represents an exception created from an <see cref="HttpResponseMessage"/>.
/// </summary>
public class ExHttpResponseMessageException : Exception
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
    /// Initializes a new instance of the <see cref="ExHttpResponseMessageException"/> class.
    /// </summary>
    /// <param name="message">A message describing the current exception.</param>
    public ExHttpResponseMessageException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExHttpResponseMessageException"/> class.
    /// </summary>
    /// <param name="message">A message describing the current exception.</param>
    /// <param name="httpResponseMessage">Source HTTP response message to source state from.</param>
    public ExHttpResponseMessageException(string? message, HttpResponseMessage httpResponseMessage) : base(message)
    {
        StatusCode = httpResponseMessage.StatusCode;
        RetryCondition = httpResponseMessage.Headers.RetryAfter;
    }

    /// <summary>
    /// Throws <see cref="AggregateException"/> containing <see cref="HttpRequestException"/> and <see cref="ExHttpResponseMessageException"/>.
    /// </summary>
    /// <param name="httpResponseMessage">HTTP response message.</param>
    public static void EnsureSuccessStatusCode(HttpResponseMessage httpResponseMessage)
    {
        try
        {
            httpResponseMessage.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException exception)
        {
            string message = exception.Message;
            throw new AggregateException(message, exception, new ExHttpResponseMessageException(message, httpResponseMessage));
        }
    }
}
