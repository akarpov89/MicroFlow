using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MicroFlow.Meta
{
  internal sealed class NodeConnector : INodeVisitor
  {
    private readonly List<StatementSyntax> myStatements;
    private readonly IVariableManager myVariableManager;

    public NodeConnector([NotNull] List<StatementSyntax> statements, [NotNull] IVariableManager variableManager)
    {
      myStatements = statements.NotNull();
      myVariableManager = variableManager.NotNull();
    }

    public void Connect([NotNull] List<NodeInfo> nodes)
    {
      foreach (var node in nodes)
      {
        node.Accept(this);
      }
    }

    public void Visit(ActivityInfo node)
    {
      ConnectNode(node);
    }

    public void Visit(ConditionInfo node)
    {
      Connect(node, node.WhenTrue, "ConnectTrueTo");
      Connect(node, node.WhenTrue, "ConnectFalseTo");
    }

    public void Visit(SwitchInfo node)
    {
      foreach (var caseInfo in node.Cases)
      {
        ConnectCase(node, caseInfo);
      }

      if (node.DefaultCase != null)
      {
        Connect(node, node.DefaultCase, "ConnectDefaultTo");
      }
    }

    public void Visit(ForkJoinInfo node)
    {
      ConnectNode(node);
    }

    public void Visit(BlockInfo node)
    {
      ConnectNormalFlow(node, node.PointsTo);
    }

    private void ConnectNode(ActivityLikeNodeInfo node)
    {
      ConnectNormalFlow(node, node.PointsTo);
      ConnectFault(node, node.FaultHandler);
      ConnectCancellation(node, node.CancellationHandler);
    }

    private void ConnectNormalFlow(NodeInfo source, NodeInfo target)
    {
      Connect(source, target, "ConnectTo");
    }

    private void ConnectFault(NodeInfo source, NodeInfo target)
    {
      Connect(source, target, "ConnectFaultTo");
    }

    private void ConnectCancellation(NodeInfo source, NodeInfo target)
    {
      Connect(source, target, "ConnectCancellationTo");
    }

    private void Connect([NotNull] NodeInfo source, [CanBeNull] NodeInfo target, [NotNull] string method)
    {
      if (target == null) return;

      var statement = ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(myVariableManager.GetVariableOf(source)),
            IdentifierName(method)))
          .WithArgumentList(
            ArgumentList(
              SingletonSeparatedList(
                Argument(
                  IdentifierName(myVariableManager.GetVariableOf(target)))))));

      myStatements.Add(statement);
    }

    private void ConnectCase([NotNull] SwitchInfo source, [NotNull] CaseInfo caseInfo)
    {
      var statement = ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            InvocationExpression(
              MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(myVariableManager.GetVariableOf(source)),
                IdentifierName("ConnectCase")))
              .WithArgumentList(
                ArgumentList(
                  SingletonSeparatedList(
                    Argument(
                      caseInfo.ValueExpression.ToExpression())))),
            IdentifierName("To")))
          .WithArgumentList(
            ArgumentList(
              SingletonSeparatedList(
                Argument(
                  IdentifierName(myVariableManager.GetVariableOf(caseInfo.Handler)))))));

      myStatements.Add(statement);
    }
  }
}