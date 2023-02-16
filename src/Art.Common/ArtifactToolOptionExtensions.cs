using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace Art.Common;

/// <summary>
/// Provides extensions for handling profile options.
/// </summary>
public static class ArtifactToolOptionExtensions
{
    private static readonly HashSet<string> s_yesLower = new() { "y", "yes", "" };

    #region Base

    /// <summary>
    /// Attempts to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located and nonnull.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON.</exception>
    public static T GetOption<T>(this IReadOnlyDictionary<string, JsonElement>? options, string optKey)
    {
        if (!(options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.Deserialize<T>(ArtJsonSerializerOptions.s_jsonOptions) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Attempts to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON.</exception>
    public static void GetOption<T>(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, ref T value, bool throwIfIncorrectType = false)
    {
        if (!(options?.TryGetValue(optKey, out JsonElement vv) ?? false)) return;
        if (vv.ValueKind == JsonValueKind.Null) throw new NullJsonDataException();
        try
        {
            value = vv.Deserialize<T>(ArtJsonSerializerOptions.s_jsonOptions) ?? throw new NullJsonDataException();
        }
        catch (JsonException)
        {
            if (throwIfIncorrectType) throw;
        }
    }

    /// <summary>
    /// Attempts to get option.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public static bool TryGetOption<T>(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, [NotNullWhen(true)] out T? value, bool throwIfIncorrectType = false)
    {
        if (options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            try
            {
                value = vv.Deserialize<T>(ArtJsonSerializerOptions.s_jsonOptions);
                return value != null;
            }
            catch (JsonException)
            {
                if (throwIfIncorrectType) throw;
            }
        }
        value = default;
        return false;
    }

    #endregion

    #region String

    /// <summary>
    /// Attempts to get option or throw exception if not found.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    public static string GetStringOption(this IReadOnlyDictionary<string, JsonElement>? options, string optKey)
    {
        if (!(options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.ToString();
    }

    /// <summary>
    /// Attempts to get option.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <exception cref="NullJsonDataException">Thrown for null JSON.</exception>
    public static void GetStringOption(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, ref string value)
    {
        if (!(options?.TryGetValue(optKey, out JsonElement vv) ?? false)) return;
        if (vv.ValueKind == JsonValueKind.Null) throw new NullJsonDataException();
        value = vv.ToString();
    }

    /// <summary>
    /// Attempts to get option.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public static bool TryGetStringOption(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, [NotNullWhen(true)] out string? value)
    {
        if (options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            value = vv.ToString();
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Gets an string option from a string value, or use a group name as fallback.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="group">Group fallback.</param>
    /// <returns>Option value.</returns>
    public static string GetStringOptionOrGroup(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, string group)
    {
        return TryGetStringOption(options, optKey, out string? optValue) ? optValue : group;
    }

    #endregion

    #region Flag

    /// <summary>
    /// Checks if a flag is true.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if flag is set to true.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static bool GetFlag(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, bool throwIfIncorrectType = false)
    {
        return TryGetOption(options, optKey, out bool? value) && value.Value
               || TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType) && s_yesLower.Contains(valueStr.ToLowerInvariant());
    }

    /// <summary>
    /// Modifies a ref bool if a flag option is present.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="flag">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static void GetFlag(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, ref bool flag, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(options, optKey, out bool? value)) flag = value.Value;
        if (TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType)) flag = s_yesLower.Contains(valueStr.ToLowerInvariant());
    }

    /// <summary>
    /// Checks if a flag is true.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="flag">Value, if found and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if value is located.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static bool TryGetFlag(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, out bool flag, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(options, optKey, out bool? value))
        {
            flag = value.Value;
            return true;
        }
        if (TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType))
        {
            flag = s_yesLower.Contains(valueStr.ToLowerInvariant());
            return true;
        }
        flag = false;
        return false;
    }

    #endregion

    #region Int

    /// <summary>
    /// Gets an Int64 option from a string or literal value.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Value.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static long GetInt64Option(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, bool throwIfIncorrectType = false)
    {
        if (!TryGetOption(options, optKey, out long valueL, throwIfIncorrectType)) throw new ArtifactToolOptionNotFoundException(optKey);
        return valueL;
    }

    /// <summary>
    /// Attempts to get an Int64 option from a string or literal value.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static void GetInt64Option(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, ref long value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(options, optKey, out long valueL)) value = valueL;
        if (TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType) && long.TryParse(valueStr, out long valueParsed)) value = valueParsed;
    }

    /// <summary>
    /// Attempts to get a Int64 option from a string or literal value.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static bool TryGetInt64Option(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, [NotNullWhen(true)] out long? value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(options, optKey, out value)) return true;
        if (TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType) && long.TryParse(valueStr, out long valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets an Int64 option from a string or literal value, or use a group name as fallback.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="group">Group fallback.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Option value.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static long GetInt64OptionOrGroup(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, string group, bool throwIfIncorrectType = false)
    {
        return TryGetInt64Option(options, optKey, out long? optValue, throwIfIncorrectType) ? optValue.Value : long.Parse(group, CultureInfo.InvariantCulture);
    }

    #endregion

    #region UInt

    /// <summary>
    /// Gets a UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Value.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static ulong GetUInt64Option(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, bool throwIfIncorrectType = false)
    {
        if (!TryGetOption(options, optKey, out ulong valueL, throwIfIncorrectType)) throw new ArtifactToolOptionNotFoundException(optKey);
        return valueL;
    }

    /// <summary>
    /// Attempts to get a UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static void GetUInt64Option(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, ref ulong value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(options, optKey, out ulong valueL)) value = valueL;
        if (TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType) && ulong.TryParse(valueStr, out ulong valueParsed)) value = valueParsed;
    }

    /// <summary>
    /// Attempts to get an UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static bool TryGetUInt64Option(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, [NotNullWhen(true)] out ulong? value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(options, optKey, out value)) return true;
        if (TryGetOption(options, optKey, out string? valueStr, throwIfIncorrectType) && ulong.TryParse(valueStr, out ulong valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a UInt64 option from a string or literal value, or use a group name as fallback.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="optKey">Key to search.</param>
    /// <param name="group">Group fallback.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Option value.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public static ulong GetUInt64OptionOrGroup(this IReadOnlyDictionary<string, JsonElement>? options, string optKey, string group, bool throwIfIncorrectType = false)
    {
        return TryGetUInt64Option(options, optKey, out ulong? optValue, throwIfIncorrectType) ? optValue.Value : ulong.Parse(group, CultureInfo.InvariantCulture);
    }

    #endregion
}
