using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Art.Common.Proxies;

internal class FindAsListTool : IArtifactToolList
{
    private readonly IArtifactToolFind _baseTool;
    private readonly IReadOnlyList<string> _ids;

    public FindAsListTool(IArtifactToolFind baseTool, IReadOnlyList<string> ids)
    {
        _baseTool = baseTool ?? throw new ArgumentNullException(nameof(baseTool));
        _ids = ids ?? throw new ArgumentNullException(nameof(ids));
    }

    public void Dispose() => _baseTool.Dispose();

    public bool DebugMode
    {
        get => _baseTool.DebugMode;
        set => _baseTool.DebugMode = value;
    }

    public IToolLogHandler? LogHandler
    {
        get => _baseTool.LogHandler;
        set => _baseTool.LogHandler = value;
    }

    public ArtifactToolProfile Profile => _baseTool.Profile;
    public ArtifactToolConfig Config => _baseTool.Config;
    public EagerFlags AllowedEagerModes => _baseTool.AllowedEagerModes;

    public IArtifactRegistrationManager RegistrationManager
    {
        get => _baseTool.RegistrationManager;
        set => _baseTool.RegistrationManager = value;
    }

    public IArtifactDataManager DataManager
    {
        get => _baseTool.DataManager;
        set => _baseTool.DataManager = value;
    }

    public JsonSerializerOptions JsonOptions
    {
        get => _baseTool.JsonOptions;
        set => _baseTool.JsonOptions = value;
    }

    public Task InitializeAsync(ArtifactToolConfig? config = null, ArtifactToolProfile? profile = null, CancellationToken cancellationToken = default) => _baseTool.InitializeAsync(config, profile, cancellationToken);

    public async IAsyncEnumerable<IArtifactData> ListAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (string id in _ids)
        {
            var artifact = await _baseTool.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (artifact != null)
            {
                yield return artifact;
            }
        }
    }
}
