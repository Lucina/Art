using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Art.M3U;

/// <summary>
/// Represents a top-down saver.
/// </summary>
public class M3UDownloaderContextTopDownSaver : M3UDownloaderContextSaverBase
{
    private static readonly Regex s_bitRegex = new(@"(^[\S\s]*[^\d]|)\d+(\.\w+)$");
    private readonly long _top;
    private readonly Func<string, long, string> _nameTransform;

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top)
        : this(context, top, TranslateNameDefault)
    {
    }

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top, Func<long, string> idFormatter)
        : this(context, top, (a, b) => TranslateNameDefault(a, idFormatter(b)))
    {
    }

    internal M3UDownloaderContextTopDownSaver(M3UDownloaderContext context, long top, Func<string, long, string> nameTransform) : base(context)
    {
        _top = top;
        _nameTransform = nameTransform;
    }

    /// <summary>
    /// Translates a generic filename.
    /// </summary>
    /// <param name="name">Donor filename.</param>
    /// <param name="i">Index.</param>
    /// <returns>Generated filename.</returns>
    /// <exception cref="InvalidDataException">Regex match failed.</exception>
    public static string TranslateNameDefault(string name, long i) => TranslateNameDefault(name, i.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Translates a generic filename.
    /// </summary>
    /// <param name="name">Donor filename.</param>
    /// <param name="i">Index.</param>
    /// <returns>Generated filename.</returns>
    /// <exception cref="InvalidDataException">Regex match failed.</exception>
    public static string TranslateNameDefault(string name, string i)
    {
        if (s_bitRegex.Match(name) is not { Success: true } bits) throw new InvalidDataException();
        return $"{bits.Groups[1].Value}{i}{bits.Groups[2].Value}";
    }

    /// <inheritdoc />
    public override async Task RunAsync(CancellationToken cancellationToken = default)
    {
        FailCounter = 0;
        long top = _top;
        while (true)
        {
            try
            {
                if (top < 0) break;
                if (HeartbeatCallback != null) await HeartbeatCallback();
                Context.Tool.LogInformation("Reading main...");
                M3UFile m3 = await Context.GetAsync(cancellationToken);
                string str = m3.DataLines.First();
                Uri origUri = new(Context.MainUri, str);
                Uri uri = new UriBuilder(new Uri(Context.MainUri, _nameTransform(str, top))) { Query = origUri.Query }.Uri;
                Context.Tool.LogInformation($"Downloading segment {uri.Segments[^1]}...");
                try
                {
                    // Don't assume MSN, and just accept failure (exception) when trying to decrypt with no IV
                    // Also don't depend on current file since it probably won't do us good anyway for this use case
                    await Context.DownloadSegmentAsync(uri, null, null, cancellationToken);
                    top--;
                }
                catch (HttpRequestException requestException)
                {
                    if (requestException.StatusCode == HttpStatusCode.NotFound)
                    {
                        Context.Tool.LogInformation("HTTP NotFound returned, ending operation");
                        return;
                    }
                    await HandleHttpRequestExceptionAsync(requestException, cancellationToken);
                }
                catch (AggregateException aggregateException)
                {
                    if (!TryGetHttpRequestException(aggregateException, out HttpRequestException? requestException, out ExHttpResponseMessageException? responseMessageException))
                        throw;
                    if (requestException.StatusCode == HttpStatusCode.NotFound)
                    {
                        Context.Tool.LogInformation("HTTP NotFound returned, ending operation");
                        return;
                    }
                    await HandleHttpRequestExceptionAsync(aggregateException.InnerExceptions, requestException, responseMessageException, cancellationToken);
                }
                await Task.Delay(500, cancellationToken);
                FailCounter = 0;
            }
            catch (HttpRequestException requestException)
            {
                await HandleHttpRequestExceptionAsync(requestException, cancellationToken);
            }
            catch (AggregateException aggregateException)
            {
                if (!TryGetHttpRequestException(aggregateException, out HttpRequestException? requestException, out ExHttpResponseMessageException? responseMessageException))
                    throw;
                await HandleHttpRequestExceptionAsync(aggregateException.InnerExceptions, requestException, responseMessageException, cancellationToken);
            }
        }
    }
}
