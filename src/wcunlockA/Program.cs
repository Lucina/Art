// dotnet publish -c Release /p:PublishSingleFile=true /p:PublishAot=true /p:DebuggerSupport=false /p:DebugType=None /p:CopyOutputSymbolsToPublishDirectory=false /p:InvariantGlobalization=true /p:OptimizationPreference=size --self-contained
using System.Security.Cryptography;

using var input = args.Length >= 1 ? new MemoryStream(File.ReadAllBytes(args[0])) : Console.OpenStandardInput();
using var output = args.Length >= 2 ? File.Create(args[1]) : Console.OpenStandardOutput();
var ms = new MemoryStream();
await input.CopyToAsync(ms);
await output.WriteAsync(ProtectedData.Unprotect(ms.ToArray(), null, DataProtectionScope.CurrentUser));
