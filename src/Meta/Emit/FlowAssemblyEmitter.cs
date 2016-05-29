using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace MicroFlow.Meta
{
  public class FlowAssemblyEmitter
  {
    private static readonly PropertyInfo LocationProperty =
      typeof(Assembly)
        .GetTypeInfo()
        .GetDeclaredProperty("Location");

    private static readonly MethodInfo AssemblyLoadRawBytes =
      typeof(Assembly)
        .GetRuntimeMethod("Load", new[] {typeof(byte[])});

    public static readonly MethodInfo AssemblyLoadByName =
      typeof(Assembly)
        .GetRuntimeMethod("Load", new[] {typeof(string)});

    private static readonly CSharpCompilationOptions DefaultCompilationOptions = 
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,        
        assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);

    [NotNull]
    public Assembly EmitAssembly([NotNull] FlowScheme scheme, [NotNull] string assemblyName)
    {
      return EmitAssembly(scheme, assemblyName, DefaultCompilationOptions);
    }

    [NotNull]
    public Assembly EmitAssembly(
      [NotNull] FlowScheme scheme, [NotNull] string assemblyName,
      [NotNull] CSharpCompilationOptions options)
    {
      byte[] rawAssembly = Emit(scheme, assemblyName, options);
      return (Assembly)AssemblyLoadRawBytes.Invoke(null, new object[] {rawAssembly});
    }

    [NotNull]
    public byte[] Emit([NotNull] FlowScheme scheme, [NotNull] string assemblyName)
    {
      return Emit(scheme, assemblyName, DefaultCompilationOptions);
    }

    [NotNull]
    public byte[] Emit(
      [NotNull] FlowScheme scheme, [NotNull] string assemblyName,
      [NotNull] CSharpCompilationOptions options)
    {
      var flowTreeBuilder = new FlowTreeBuilder(scheme);

      var syntaxTree = flowTreeBuilder.Build();

      var references = new ReferencesCollector().Collect(scheme);

      var assemblyRuntime = (Assembly)AssemblyLoadByName.Invoke(null,
        new object[] { "System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" });

      if (assemblyRuntime != null)
      {
        references.Add(assemblyRuntime);
      }

      var metadataReferences = references.Select(r => MetadataReference.CreateFromFile((string)LocationProperty.GetValue(r)));

      var compilation = CSharpCompilation.Create(
        assemblyName,
        new[] { syntaxTree },
        metadataReferences,
        options);

      using (var ms = new MemoryStream())
      {
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
          IEnumerable<Diagnostic> failures = result.Diagnostics
            .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

          var errors = new List<string>();

          foreach (Diagnostic diagnostic in failures)
          {
            errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
          }

          throw new FlowCompilationException(errors.ToArray());
        }

        ms.Seek(0, SeekOrigin.Begin);
        return ms.ToArray();
      }
    }
  }
}