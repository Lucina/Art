/*
 * https://github.com/Riboe/M3USharp/blob/master/M3UParser/M3UReader.cs
 * @b7a8ebc
 * MIT-licensed by Riboe
 */

using System.Globalization;
using System.Text;

namespace Art.M3U;

/// <summary>
/// Provides M3U reading functionality.
/// </summary>
public static class M3UReader
{
    private static readonly CultureInfo s_culture = CultureInfo.InvariantCulture;
    private static Dictionary<string, string> TmpDict => _tmpDict ??= new Dictionary<string, string>();
    [ThreadStatic] private static Dictionary<string, string>? _tmpDict;

    private const string FILE_HEADER = "#EXTM3U";
    private const string TAG_VERSION = "#EXT-X-VERSION:";
    private const string TAG_STREAM_INFO = "#EXT-X-STREAM-INF:";
    private const string TAG_KEY = "#EXT-X-KEY:";
    private const string TAG_MEDIA_SEQUENCE = "#EXT-X-MEDIA-SEQUENCE:";
    private const string STREAM_INF_BANDWIDTH = "BANDWIDTH";
    private const string STREAM_INF_AVERAGE_BANDWIDTH = "AVERAGE-BANDWIDTH";
    private const string STREAM_INF_NAME = "NAME";
    private const string STREAM_INF_CODECS = "CODECS";
    private const string STREAM_INF_RESOLUTION = "RESOLUTION";
    private const string ENCRYPTION_INF_METHOD = "METHOD";
    private const string ENCRYPTION_INF_KEYFORMAT = "KEYFORMAT";
    private const string ENCRYPTION_INF_URI = "URI";
    private const string ENCRYPTION_INF_IV = "IV";
    private static readonly string[] Tags = { TAG_VERSION, TAG_STREAM_INFO, TAG_KEY, TAG_MEDIA_SEQUENCE };

    /// <summary>
    /// Parses M3U content.
    /// </summary>
    /// <param name="data">Text content.</param>
    /// <returns>Parsed structure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown for incorrect M3U structure.</exception>
    public static M3UFile Parse(string data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        using StringReader reader = new(data);
        string? line = reader.ReadLine();
        if (!FILE_HEADER.Equals(line)) throw new InvalidDataException("Invalid header.");

        M3UFile result = new();

        string? previousLine = null;
        while ((line = reader.ReadLine()) != null)
        {
            string? tag = GetTag(line);
            if (tag != null)
            {
                SetTagValue(result, tag, line);
            }
            else if (previousLine != null && previousLine.StartsWith(TAG_STREAM_INFO))
            {
                result.Streams[^1].Path = line;
            }
            else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'))
                result.DataLines.Add(line);

            previousLine = line;
        }

        return result;
    }

    private static void ParseStreamInfo(M3UFile file, string tag, string line)
    {
        StreamInfo stream = new();
        try
        {
            ParseKvsToDictionary(tag, line, TmpDict);
            if (TmpDict.TryGetValue(STREAM_INF_BANDWIDTH, out string? bandWidth))
                stream.Bandwidth = long.Parse(bandWidth, s_culture);
            if (TmpDict.TryGetValue(STREAM_INF_AVERAGE_BANDWIDTH, out string? averageBandWidth))
                stream.AverageBandwidth = long.Parse(averageBandWidth, s_culture);
            if (TmpDict.TryGetValue(STREAM_INF_NAME, out string? name))
                stream.Name = name;
            if (TmpDict.TryGetValue(STREAM_INF_CODECS, out string? codecs))
                stream.Codecs = codecs;
            if (TmpDict.TryGetValue(STREAM_INF_RESOLUTION, out string? resolution))
            {
                string[] split = resolution.Split('x');
                stream.ResolutionWidth = int.Parse(split[0], s_culture);
                stream.ResolutionHeight = int.Parse(split[1], s_culture);
            }
            file.AddStream(stream);
        }
        finally
        {
            TmpDict.Clear();
        }
    }

    private static void ParseKey(M3UFile file, string tag, string line)
    {
        string? keyFormat = null;
        string? method = null;
        string? uri = null;
        byte[]? iv = null;
        try
        {
            ParseKvsToDictionary(tag, line, TmpDict);
            if (TmpDict.TryGetValue(ENCRYPTION_INF_KEYFORMAT, out string? keyFormatT)) keyFormat = keyFormatT;
            if (TmpDict.TryGetValue(ENCRYPTION_INF_METHOD, out string? methodT)) method = methodT;
            if (TmpDict.TryGetValue(ENCRYPTION_INF_URI, out string? uriT)) uri = uriT;
            if (TmpDict.TryGetValue(ENCRYPTION_INF_IV, out string? ivT))
            {
                ReadOnlySpan<char> ivS = ivT;
                if (ivS.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) ivS = ivS[2..];
                iv = Convert.FromHexString(ivS);
            }
            keyFormat ??= "identity";
            if (method == null) throw new InvalidDataException("method not provided");
            file.EncryptionInfo = new M3UEncryptionInfo(keyFormat, method, uri, null, iv);
        }
        finally
        {
            TmpDict.Clear();
        }
    }

    private static void ParseKvsToDictionary(string tag, string line, Dictionary<string, string> dict)
    {
        StringBuilder builder = new();
        int quoteCount = 0;
        string? currentStreamTag = null;

        string data = line[tag.Length..];
        for (int i = 0; i <= data.Length; i++)
        {
            char? c = i < data.Length ? data[i] : null;

            if (c == '"')
            {
                quoteCount++;
            }
            else if (c == '=' && quoteCount % 2 == 0)
            {
                currentStreamTag = builder.ToString();
                builder.Clear();
            }
            else if (c == ',' && quoteCount % 2 == 0 || c == null)
            {
                string value = builder.ToString();
                builder.Clear();
                if (currentStreamTag == null) throw new InvalidDataException();
                dict[currentStreamTag] = value;
            }
            else
            {
                builder.Append(c);
            }
        }
    }

    /// <summary>
    /// Extracts the value of the given tag from the given line and sets the corresponding property of the file to the
    /// extracted value.
    /// </summary>
    private static void SetTagValue(M3UFile file, string tag, string line)
    {
        string value = line[tag.Length..];
        switch (tag)
        {
            case TAG_VERSION:
                file.Version = value;
                break;
            case TAG_STREAM_INFO:
                ParseStreamInfo(file, tag, line);
                break;
            case TAG_KEY:
                ParseKey(file, tag, line);
                break;
            case TAG_MEDIA_SEQUENCE:
                file.FirstMediaSequenceNumber = long.Parse(value, CultureInfo.InvariantCulture);
                break;
        }
    }

    /// <summary>
    /// Checks if the given string starts with one of the supported tags and returns the tag. Otherwise, returns null.
    /// </summary>
    private static string? GetTag(string line)
    {
        return Tags.FirstOrDefault(line.StartsWith);
    }
}
