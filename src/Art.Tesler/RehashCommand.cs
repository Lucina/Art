using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Security.Cryptography;
using Art.Common;

namespace Art.Tesler;

internal class RehashCommand : CommandBase
{
    protected ITeslerDataProvider DataProvider;

    protected ITeslerRegistrationProvider RegistrationProvider;

    protected Option<string> HashOption;

    protected Option<bool> DetailedOption;

    public RehashCommand(
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider)
        : this(dataProvider, registrationProvider, "rehash", "Recompute hashes for archive contents.")
    {
    }

    public RehashCommand(
        ITeslerDataProvider dataProvider,
        ITeslerRegistrationProvider registrationProvider,
        string name,
        string? description = null) : base(name, description)
    {
        DataProvider = dataProvider;
        DataProvider.Initialize(this);
        RegistrationProvider = registrationProvider;
        RegistrationProvider.Initialize(this);
        HashOption = new Option<string>(new[] { "-h", "--hash" }, $"Checksum algorithm ({Common.ChecksumAlgorithms})") { IsRequired = true };
        AddOption(HashOption);
        DetailedOption = new Option<bool>(new[] { "--detailed" }, "Show detailed information on entries");
        AddOption(DetailedOption);
    }

    protected override async Task<int> RunAsync(InvocationContext context)
    {
        string hash = context.ParseResult.GetValueForOption(HashOption)!;
        if (!ChecksumSource.DefaultSources.ContainsKey(hash))
        {
            PrintErrorMessage(Common.GetInvalidHashMessage(hash), context.Console);
            return 2;
        }
        using var adm = DataProvider.CreateArtifactDataManager(context);
        using var arm = RegistrationProvider.CreateArtifactRegistrationManager(context);
        Dictionary<ArtifactKey, List<ArtifactResourceInfo>> failed = new();
        int rehashed = 0;

        void AddFail(ArtifactResourceInfo r)
        {
            if (!failed.TryGetValue(r.Key.Artifact, out var list)) list = failed[r.Key.Artifact] = new List<ArtifactResourceInfo>();
            list.Add(r);
        }

        bool detailed = context.ParseResult.GetValueForOption(DetailedOption);
        foreach (ArtifactInfo inf in await arm.ListArtifactsAsync())
        foreach (ArtifactResourceInfo rInf in await arm.ListResourcesAsync(inf.Key))
        {
            if (rInf.Checksum == null || !ChecksumSource.DefaultSources.TryGetValue(rInf.Checksum.Id, out ChecksumSource? haOriginalV))
                continue;
            using HashAlgorithm haOriginal = haOriginalV.HashAlgorithmFunc!();
            if (!await adm.ExistsAsync(rInf.Key))
            {
                AddFail(rInf);
                continue;
            }
            if (!ChecksumSource.DefaultSources.TryGetValue(hash, out ChecksumSource? haNewV))
            {
                PrintErrorMessage($"Failed to instantiate new hash algorithm for {hash}", context.Console);
                return 2;
            }
            Common.PrintFormat(rInf.GetInfoPathString(), detailed, () => rInf.GetInfoString(), context.Console);
            using HashAlgorithm haNew = haNewV.HashAlgorithmFunc!();
            await using Stream sourceStream = await adm.OpenInputStreamAsync(rInf.Key);
            await using HashProxyStream hpsOriginal = new(sourceStream, haOriginal, true, true);
            await using HashProxyStream hpsNew = new(hpsOriginal, haNew, true, true);
            await using MemoryStream ms = new();
            await hpsNew.CopyToAsync(ms);
            if (!rInf.Checksum.Value.AsSpan().SequenceEqual(hpsOriginal.GetHash()))
            {
                AddFail(rInf);
                continue;
            }
            ArtifactResourceInfo nInf = rInf with { Checksum = new Checksum(haNewV.Id, hpsNew.GetHash()) };
            await arm.AddResourceAsync(nInf);
            Common.PrintFormat(nInf.GetInfoPathString(), detailed, () => nInf.GetInfoString(), context.Console);
            rehashed++;
        }
        context.Console.Out.WriteLine();
        if (failed.Count != 0)
        {
            PrintErrorMessage($"{failed.Sum(v => v.Value.Count)} resources with checksums failed validation before rehash.", context.Console);
            foreach (ArtifactResourceInfo value in failed.Values.SelectMany(v => v)) Common.Display(value, detailed, context.Console);
            return 1;
        }
        context.Console.Out.WriteLine($"{rehashed} resources successfully rehashed.");
        return 0;
    }
}
