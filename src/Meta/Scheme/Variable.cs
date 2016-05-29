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
      Bindings = new List<VariableBindingInfo>();
    }

    [NotNull]
    public Type Type { get; }

    [NotNull]
    public string Name { get; }

    [CanBeNull]
    public string InitialValueExpression { get; set; }

    [NotNull]
    public List<VariableBindingInfo> Bindings { get; }

    [NotNull]
    public VariableInfo AddBinding([NotNull] VariableBindingInfo binding)
    {
      Bindings.Add(binding.NotNull());
      return this;
    }
  }

  public class VariableBindingInfo
  {
    public VariableBindingInfo(ActivityInfo activity, VariableBindingKind bindingKind)
    {
      Activity = activity.NotNull();
      Kind = bindingKind;
    }

    [NotNull]
    public ActivityInfo Activity { get; }

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