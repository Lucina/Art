using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace Art;

public partial class ArtifactTool
{
    #region Options

    /// <summary>
    /// Attempt to get option or throw exception if not found.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    public string GetStringOptionOrExcept(string optKey)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.ToString();
    }

    /// <summary>
    /// Attempt to get option or throw exception if not found or if null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Value, if located and nonnull.</returns>
    /// <exception cref="ArtifactToolOptionNotFoundException">Thrown when option is not found.</exception>
    /// <exception cref="JsonException">Thrown when conversion failed.</exception>
    /// <exception cref="NotSupportedException">Thrown when type not supported.</exception>
    public T GetOptionOrExcept<T>(string optKey)
    {
        if (!(Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)) throw new ArtifactToolOptionNotFoundException(optKey);
        return vv.Deserialize<T>(ArtJsonSerializerOptions.s_jsonOptions) ?? throw new NullJsonDataException();
    }

    /// <summary>
    /// Attempt to get option.
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

    /// <summary>
    /// Attempt to get option.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> if type is wrong.</param>
    /// <returns>True if value is located and of the right type.</returns>
    public bool TryGetStringOption(string optKey, [NotNullWhen(true)] out string? value, bool throwIfIncorrectType = false)
    {
        if (Profile.Options?.TryGetValue(optKey, out JsonElement vv) ?? false)
        {
            try
            {
                value = vv.ToString();
                return true;
            }
            catch (JsonException)
            {
                if (throwIfIncorrectType) throw;
            }
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Checks if a flag is true.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if flag is set to true.</returns>
    public bool GetFlagTrue(string optKey, bool throwIfIncorrectType = false)
    {
        return TryGetOption(optKey, out bool? value, throwIfIncorrectType) && value.Value
               || TryGetOption(optKey, out string? valueStr) && s_yesLower.Contains(valueStr.ToLowerInvariant());
    }

    /// <summary>
    /// Modifies a ref bool if a flag option is present.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="flag">Flag to set.</param>
    /// <param name="throwIfIncorrectType">If true, throw a <see cref="JsonException"/> or <see cref="NotSupportedException"/> if type is wrong.</param>
    /// <returns>True if flag is set to true.</returns>
    public void SetFlag(string optKey, ref bool flag, bool throwIfIncorrectType = false)
    {
        if (TryGetOption(optKey, out bool? value, throwIfIncorrectType)) flag = value.Value;
        if (TryGetOption(optKey, out string? valueStr)) flag = s_yesLower.Contains(valueStr.ToLowerInvariant());
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

    /// <summary>
    /// Gets an Int64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    public long GetInt64OptionOrGroup(string optKey)
    {
        return TryGetInt64Option(optKey, out long? optValue) ? optValue.Value : long.Parse(Profile.Group, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets an UInt64 option from a string or literal value, or parses value from <see cref="ArtifactToolProfile.Group"/>.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <returns>Option value.</returns>
    public ulong GetUInt64OptionOrGroup(string optKey)
    {
        return TryGetUInt64Option(optKey, out ulong? optValue) ? optValue.Value : ulong.Parse(Profile.Group, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Attempts to get an Int64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if found.</returns>
    public bool TryGetInt64Option(string optKey, [NotNullWhen(true)] out long? value)
    {
        if (TryGetOption(optKey, out value)) return true;
        if (TryGetOption(optKey, out string? valueStr) && long.TryParse(valueStr, out long valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to get an UInt64 option from a string or literal value.
    /// </summary>
    /// <param name="optKey">Key to search.</param>
    /// <param name="value">Value, if located and nonnull.</param>
    /// <returns>True if found.</returns>
    public bool TryGetUInt64Option(string optKey, [NotNullWhen(true)] out ulong? value)
    {
        if (TryGetOption(optKey, out value)) return true;
        if (TryGetOption(optKey, out string? valueStr) && ulong.TryParse(valueStr, out ulong valueParsed))
        {
            value = valueParsed;
            return true;
        }
        return false;
    }

    #endregion
}
