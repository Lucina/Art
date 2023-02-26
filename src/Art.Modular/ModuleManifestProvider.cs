using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace Art.Modular;

public class ModuleManifestProvider
{
    private readonly Dictionary<string, ModuleManifest> _manifests = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly HashSet<string> _searched = new();
    private readonly string _pluginDirectory;
    private readonly string _directorySuffix;
    private readonly string _fileNameSuffix;

    public static ModuleManifestProvider CreateDefault(string directorySuffix = ".kix", string fileNameSuffix = ".kix.json")
    {
        return Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"), directorySuffix, fileNameSuffix);
    }

    public static ModuleManifestProvider Create(string pluginDirectory, string directorySuffix, string fileNameSuffix)
    {
        return new ModuleManifestProvider(pluginDirectory, directorySuffix, fileNameSuffix);
    }

    private ModuleManifestProvider(string pluginDirectory, string directorySuffix, string fileNameSuffix)
    {
        _pluginDirectory = pluginDirectory;
        _directorySuffix = directorySuffix;
        _fileNameSuffix = fileNameSuffix;
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    public bool TryFind(string assembly, [NotNullWhen(true)] out ModuleManifest? manifest)
    {
        if (_manifests.TryGetValue(assembly, out manifest))
        {
            return true;
        }
        if (!Directory.Exists(_pluginDirectory))
        {
            manifest = null;
            return false;
        }
        if (TryFind(assembly, _pluginDirectory, out manifest, _manifests, _searched))
        {
            return true;
        }
        manifest = null;
        return false;
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    public void LoadManifests(IDictionary<string, ModuleManifest> dictionary)
    {
        if (!Directory.Exists(_pluginDirectory)) return;
        LoadManifests(dictionary, _pluginDirectory, _searched);
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    public IArtifactToolRegistry LoadForManifest(ModuleManifest manifest)
    {
        string baseDir = manifest.Content.Path != null && !Path.IsPathFullyQualified(manifest.Content.Path) ? Path.Combine(manifest.BasePath, manifest.Content.Path) : manifest.BasePath;
        var ctx = new ArtModuleAssemblyLoadContext(baseDir, manifest.Content.Assembly);
        return new PluginWithManifest(manifest, ctx, ctx.LoadFromAssemblyName(new AssemblyName(manifest.Content.Assembly)));
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    private bool TryFind(string assembly, string dir, [NotNullWhen(true)] out ModuleManifest? manifest, IDictionary<string, ModuleManifest>? toAugment = null, ISet<string>? searched = null)
    {
        foreach (string directory in Directory.EnumerateDirectories(dir, $"*{_directorySuffix}", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }))
        {
            if (searched != null && !searched.Add(Path.GetFullPath(directory)))
            {
                continue;
            }
            if (TryFindAtTarget(assembly, directory, out manifest, toAugment))
            {
                return true;
            }
        }
        manifest = null;
        return false;
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    private bool TryFindAtTarget(string assembly, string directory, [NotNullWhen(true)] out ModuleManifest? manifest, IDictionary<string, ModuleManifest>? toAugment = null)
    {
        foreach (string file in Directory.EnumerateFiles(directory, $"*{_fileNameSuffix}", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }))
        {
            if (TryLoad(file, out var content))
            {
                manifest = null;
                if (toAugment != null && !toAugment.ContainsKey(content.Assembly))
                {
                    manifest = new ModuleManifest(directory, content);
                    toAugment.Add(content.Assembly, manifest);
                }
                if (content.Assembly.Equals(assembly, StringComparison.InvariantCultureIgnoreCase))
                {
                    manifest ??= new ModuleManifest(directory, content);
                    return true;
                }
            }
        }
        manifest = null;
        return false;
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    private void LoadManifests(IDictionary<string, ModuleManifest> dictionary, string dir, ISet<string>? searched = null)
    {
        foreach (string directory in Directory.EnumerateDirectories(dir, $"*{_directorySuffix}", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }))
        {
            if (searched != null && !searched.Add(Path.GetFullPath(directory)))
            {
                continue;
            }
            LoadManifestsAtTarget(dictionary, directory);
        }
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    private void LoadManifestsAtTarget(IDictionary<string, ModuleManifest> dictionary, string directory)
    {
        foreach (string file in Directory.EnumerateFiles(directory, $"*{_fileNameSuffix}", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }))
        {
            if (TryLoad(file, out var content) && !dictionary.ContainsKey(content.Assembly))
            {
                dictionary.Add(content.Assembly, new ModuleManifest(directory, content));
            }
        }
    }

    [RequiresUnreferencedCode("Loading artifact tools might require types that cannot be statically analyzed.")]
    private static bool TryLoad(string file, [NotNullWhen(true)] out ModuleManifestContent? content)
    {
        try
        {
            content = JsonSerializer.Deserialize<ModuleManifestContent>(File.ReadAllText(file)) ?? throw new IOException($"Failed to deserialize manifest file {file}");
            return true;
        }
        catch
        {
            content = null;
            return false;
        }
    }
}
