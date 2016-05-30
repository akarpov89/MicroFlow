using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MicroFlow.Meta
{
  internal sealed class Binder : INodeVisitor
  {
    private readonly List<StatementSyntax> myStatements;
    private readonly IVariableManager myVariableManager;

    public Binder([NotNull] List<StatementSyntax> statements, [NotNull] IVariableManager variableManager)
    {
      myStatements = statements.NotNull();
      myVariableManager = variableManager.NotNull();
    }

    public void Bind([NotNull] List<NodeInfo> nodes)
    {
      foreach (var node in nodes)
      {
        node.Accept(this);
      }
    }

    public void Visit(ActivityInfo node)
    {
      foreach (var binding in node.PropertyBindings)
      {
        switch (binding.Kind)
        {
          case PropertyBindingKind.ActivityResult:
            AddBindingStatement(node, binding.Property, "ToResultOf", binding.Activity);
            break;
          case PropertyBindingKind.ActivityException:
            AddBindingStatement(node, binding.Property, "ToExceptionOf", binding.Activity);
            break;
          case PropertyBindingKind.Expression:
            AddBindingStatement(node, binding.Property, "To", binding.BindingExpression);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      foreach (var binding in node.VariableBindings)
      {
        switch (binding.Kind)
        {
          case VariableBindingKind.ActivityResult:
            AddResultVariableBinding(binding.Variable, node);
            break;
          case VariableBindingKind.AssignExpression:
            AddActivityCompletionBinding(node, binding.Variable, "OnCompletionAssign", binding.BindingExpression);
            break;
          case VariableBindingKind.UpdateExpression:
            AddActivityCompletionBinding(node, binding.Variable, "OnCompletionUpdate", binding.BindingExpression);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public void Visit(ConditionInfo node)
    {
    }

    public void Visit(SwitchInfo node)
    {
    }

    public void Visit(ForkJoinInfo node)
    {
      foreach (var fork in node.Forks)
      {
        Visit(fork);
      }
    }

    public void Visit(BlockInfo node)
    {
    }

    private void AddBindingStatement(NodeInfo node, string property, string bindingMethod, NodeInfo bindingSource)
    {
      AddBindingStatement(node, property, bindingMethod, IdentifierName(myVariableManager.GetVariableOf(bindingSource)));
    }

    private void AddBindingStatement(NodeInfo node, string property, string bindingMethod, string expression)
    {
      AddBindingStatement(node, property, bindingMethod, expression.ToExpression());
    }

    private void AddBindingStatement(NodeInfo node, string property, string bindingMethod, ExpressionSyntax expression)
    {
      var statement = ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            GetBindInvocation(node, property),
            IdentifierName(bindingMethod)))
          .WithArgumentList(
            ArgumentList(
              SingletonSeparatedList(
                Argument(expression)))));

      myStatements.Add(statement);
    }

    private InvocationExpressionSyntax GetBindInvocation(NodeInfo node, string property)
    {
      return InvocationExpression(
        MemberAccessExpression(
          SyntaxKind.SimpleMemberAccessExpression,
          IdentifierName(myVariableManager.GetVariableOf(node)),
          IdentifierName("Bind")))
        .WithArgumentList(
          ArgumentList(
            SingletonSeparatedList(
              Argument(
                SimpleLambdaExpression(
                  Parameter(
                    Identifier("__act__")),
                  MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("__act__"),
                    IdentifierName(property)))))));
    }

    private void AddResultVariableBinding(VariableInfo variable, ActivityInfo activity)
    {
      var statement = ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(variable.Name),
            IdentifierName("BindToResultOf")))
          .WithArgumentList(
            ArgumentList(
              SingletonSeparatedList(
                Argument(
                  IdentifierName(myVariableManager.GetVariableOf(activity)))))));

      myStatements.Add(statement);
    }

    private void AddActivityCompletionBinding(
      ActivityInfo activity, VariableInfo variable, string method, string expression)
    {
      var statement = ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(myVariableManager.GetVariableOf(activity)),
            IdentifierName(method)))
          .WithArgumentList(
            ArgumentList(
              SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                  Argument(
                    IdentifierName(variable.Name)),
                  Token(SyntaxKind.CommaToken),
                  Argument(expression.ToExpression())
                }))));

      myStatements.Add(statement);
    }
  }
}