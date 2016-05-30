using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public class VariableInfo
  {
    public VariableInfo([NotNull] Type variableType, [NotNull] string variableName)
    {
      Type = variableType.NotNull();
      Name = variableName.NotNull();
    }

    [NotNull]
    public Type Type { get; }

    [NotNull]
    public string Name { get; }

    [CanBeNull]
    public string InitialValueExpression { get; set; }
  }

  public class VariableBindingInfo
  {
    public VariableBindingInfo(VariableInfo variable, VariableBindingKind bindingKind)
    {
      Variable = variable.NotNull();
      Kind = bindingKind;
    }

    [NotNull]
    public VariableInfo Variable { get; }

    public VariableBindingKind Kind { get; set; }

    [CanBeNull]
    public string BindingExpression { get; set; }
  }

  public enum VariableBindingKind
  {
    ActivityResult,
    AssignExpression,
    UpdateExpression
  }
}