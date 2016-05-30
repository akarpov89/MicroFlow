using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace MicroFlow.Meta
{
  public static class FlowEmitter
  {
    private static readonly PropertyInfo LocationProperty =
      typeof(Assembly)
        .GetTypeInfo()
        .GetDeclaredProperty("Location");

    private static readonly MethodInfo AssemblyLoadRawBytes =
      typeof(Assembly)
        .GetRuntimeMethod("Load", new[] {typeof(byte[])});

    private static readonly CSharpCompilationOptions DefaultCompilationOptions =
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
        assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);

    [NotNull]
    public static Flow<TResult> EmitFlow<TResult>([NotNull] this FlowScheme scheme)
    {
      var assembly = EmitAssembly(scheme);
      var flowType = assembly.GetType(scheme.FlowFullTypeName);

      return (Flow<TResult>) Activator.CreateInstance(flowType);
    }

    [NotNull]
    public static Flow EmitFlow([NotNull] this FlowScheme scheme)
    {
      var assembly = EmitAssembly(scheme);
      var flowType = assembly.GetType(scheme.FlowFullTypeName);

      return (Flow) Activator.CreateInstance(flowType);
    }

    [NotNull]
    public static Assembly EmitAssembly(
      [NotNull] this FlowScheme scheme, [CanBeNull] string assemblyName = null,
      [CanBeNull] CSharpCompilationOptions options = null)
    {
      byte[] rawAssembly = EmitRawAssembly(scheme, assemblyName, options);
      return (Assembly) AssemblyLoadRawBytes.Invoke(null, new object[] {rawAssembly});
    }

    [NotNull]
    public static byte[] EmitRawAssembly(
      [NotNull] this FlowScheme scheme, [CanBeNull] string assemblyName = null,
      [CanBeNull] CSharpCompilationOptions options = null)
    {
      var flowTreeBuilder = new FlowTreeBuilder(scheme);

      var syntaxTree = flowTreeBuilder.Build();

      var references = new ReferencesCollector().Collect(scheme);

      var metadataReferences = references.Select(r => MetadataReference.CreateFromFile((string)LocationProperty.GetValue(r)));

      var compilation = CSharpCompilation.Create(
        assemblyName ?? GetTempAssemblyName(),
        new[] { syntaxTree },
        metadataReferences,
        options ?? DefaultCompilationOptions);

      using (var ms = new MemoryStream())
      {
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
          var failures = result.Diagnostics
            .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

          var errorMessages = failures.Select(diagnostic => $"{diagnostic.Id}: {diagnostic.GetMessage()}");

          throw new FlowCompilationException(errorMessages.ToArray());
        }

        ms.Seek(0, SeekOrigin.Begin);
        return ms.ToArray();
      }
    }

    [NotNull, Pure]
    private static string GetTempAssemblyName() => "TempAssemlby" + Guid.NewGuid().ToString("N") + ".dll";
  }
}