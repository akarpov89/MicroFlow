using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public class FlowScheme
  {
    public FlowScheme([NotNull] string description, [NotNull] string flowFullTypeName)
    {
      Description = description.NotNull();
      FlowFullTypeName = flowFullTypeName.NotNull();

      Nodes = new List<NodeInfo>();
      GlobalVariables = new List<VariableInfo>();
      Namespaces = new List<string>
      {
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "MicroFlow"
      };
      Properties = new List<FlowPropertyInfo>();
      Services = new List<ServiceInfo>();
      Validators = new List<Type>();
    }

    [NotNull]
    public string Description { get; }

    [NotNull]
    public string FlowFullTypeName { get; }

    [CanBeNull]
    public Type ResultType { get; set; }

    [NotNull]
    public List<NodeInfo> Nodes { get; }

    [CanBeNull]
    public NodeInfo IntialNode { get; set; }

    [CanBeNull]
    public Type DefaultFaultHandlerType { get; set; }

    [CanBeNull]
    public Type DefaultCancellationHandlerType { get; set; }

    [NotNull]
    public List<VariableInfo> GlobalVariables { get; }

    [NotNull]
    public List<string> Namespaces { get; }

    [NotNull]
    public List<FlowPropertyInfo> Properties { get; }

    [NotNull]
    public List<ServiceInfo> Services { get; }

    [NotNull]
    public List<Type> Validators { get; }

    [CanBeNull]
    public LoggerInfo Logger { get; set; }

    [NotNull]
    public FlowScheme AddNode([NotNull] NodeInfo nodeInfo)
    {
      Nodes.Add(nodeInfo.NotNull());
      return this;
    }

    [NotNull]
    public FlowScheme AddNodes([NotNull] params NodeInfo[] nodes)
    {
      nodes.AssertNotNull("nodes != null");

      foreach (var node in nodes)
      {
        Nodes.Add(node.NotNull());
      }
      
      return this;
    }

    [NotNull]
    public FlowScheme AddVariable([NotNull] VariableInfo variable)
    {
      GlobalVariables.Add(variable.NotNull());
      return this;
    }

    [NotNull]
    public FlowScheme AddNamespace([NotNull] string ns)
    {
      Namespaces.Add(ns.NotNull());
      return this;
    }

    [NotNull]
    public FlowScheme AddProperty([NotNull] FlowPropertyInfo property)
    {
      Properties.Add(property.NotNull());
      return this;
    }

    [NotNull]
    public FlowScheme AddProperty([NotNull] Type type, [NotNull] string name)
    {
      return AddProperty(new FlowPropertyInfo(type, name));
    }

    [NotNull]
    public FlowScheme AddProperty<TProperty>([NotNull] string name)
    {
      return AddProperty(new FlowPropertyInfo(typeof(TProperty), name));
    }

    [NotNull]
    public FlowScheme AddService([NotNull] ServiceInfo service)
    {
      Services.Add(service.NotNull());
      return this;
    }

    [NotNull]
    public FlowScheme AddValidator([NotNull] Type validatorType)
    {
      Validators.Add(validatorType.NotNull());
      return this;
    }
  }
}
