using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace Art;

public partial class ArtifactTool
{
    #region Options

    #region Base

    /// <summary>
    /// Attempts to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located and nonnull.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON.</exception>
    public T GetOption<T>(string optKey)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.Deserialize<T>(ArtJsonSerializerOptions.s_jsonOptions) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Attempts to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    /// <exception cref="NullJsonDataException">Thrown for null JSON.</exception>
    public void GetOption<T>(string optKey, ref T value, bool throwIfIncorrectType = false)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) return;
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
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public bool TryGetOption<T>(string optKey, [NotNullWhen(true)] out T? value, bool throwIfIncorrectType = false)
    {
        if (Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)
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
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    public string GetStringOption(string optKey)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.ToString();
    }

    /// <summary>
    /// Attempts to get option.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <exception cref="NullJsonDataException">Thrown for null JSON.</exception>
    public void GetStringOption(string optKey, ref string value)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) return;
        if (vv.ValueKind == JsonValueKind.Null) throw new NullJsonDataException();
        value = vv.ToString();
    }

    /// <summary>
    /// Attempts to get option.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public bool TryGetStringOption(string optKey, [NotNullWhen(true)] out string? value)
    {
        if (Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            value = vv.ToString();
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Gets an string option from a string value, or take value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    public string GetStringOptionOrGroup(string optKey)
    {
        return TryGetStringOption(optKey, out string? optValue) ? optValue : Profile.Group;
    }

    #endregion

    #region Flag

    /// <summary>
    /// Checks if a flag is true.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if flag is set to true.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public bool GetFlag(string optKey, bool throwIfIncorrectType = false)
    {
        return TryGetOption(optKey, out bool? value) && value.Value
               || TryGetOption(optKey, out string? valueStr, throwIfIncorrectType) && s_yesLower.Contains(valueStr.ToLowerInvariant());
    }

    /// <summary>
    /// Modifies a ref bool if a flag option is present.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="flag">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public void GetFlag(string optKey, ref bool flag, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out bool? value)) flag = value.Value;
        if (TryGetOption(optKey, out string? valueStr, throwIfIncorrectType)) flag = s_yesLower.Contains(valueStr.ToLowerInvariant());
    }

    /// <summary>
    /// Checks if a flag is true.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="flag">Value, if found and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if value is located.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public bool TryGetFlag(string optKey, out bool flag, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out bool? value))
        {
            flag = value.Value;
            return true;
        }
        if (TryGetOption(optKey, out string? valueStr, throwIfIncorrectType))
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
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Value.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public long GetInt64Option(string optKey, bool throwIfIncorrectType = false)
    {
        if (!TryGetOption(optKey, out long valueL, throwIfIncorrectType)) throw new ArtifactToolOptionNotFoundException(optKey);
        return valueL;
    }

    /// <summary>
    /// Attempts to get an Int64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public void GetInt64Option(string optKey, ref long value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out long valueL)) value = valueL;
        if (TryGetOption(optKey, out string? valueStr, throwIfIncorrectType) && long.TryParse(valueStr, out long valueParsed)) value = valueParsed;
    }

    /// <summary>
    /// Attempts to get a Int64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public bool TryGetInt64Option(string optKey, [NotNullWhen(true)] out long? value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out value)) return true;
        if (TryGetOption(optKey, out string? valueStr, throwIfIncorrectType) && long.TryParse(valueStr, out long valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets an Int64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Option value.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public long GetInt64OptionOrGroup(string optKey, bool throwIfIncorrectType = false)
    {
        return TryGetInt64Option(optKey, out long? optValue, throwIfIncorrectType) ? optValue.Value : long.Parse(Profile.Group, CultureInfo.InvariantCulture);
    }

    #endregion

    #region UInt

    /// <summary>
    /// Gets a UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Value.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public ulong GetUInt64Option(string optKey, bool throwIfIncorrectType = false)
    {
        if (!TryGetOption(optKey, out ulong valueL, throwIfIncorrectType)) throw new ArtifactToolOptionNotFoundException(optKey);
        return valueL;
    }

    /// <summary>
    /// Attempts to get a UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public void GetUInt64Option(string optKey, ref ulong value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out ulong valueL)) value = valueL;
        if (TryGetOption(optKey, out string? valueStr, throwIfIncorrectType) && ulong.TryParse(valueStr, out ulong valueParsed)) value = valueParsed;
    }

    /// <summary>
    /// Attempts to get an UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if found.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public bool TryGetUInt64Option(string optKey, [NotNullWhen(true)] out ulong? value, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out value)) return true;
        if (TryGetOption(optKey, out string? valueStr, throwIfIncorrectType) && ulong.TryParse(valueStr, out ulong valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a UInt64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>Option value.</returns>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public ulong GetUInt64OptionOrGroup(string optKey, bool throwIfIncorrectType = false)
    {
        return TryGetUInt64Option(optKey, out ulong? optValue, throwIfIncorrectType) ? optValue.Value : ulong.Parse(Profile.Group, CultureInfo.InvariantCulture);
    }

    #endregion

    #endregion
}
