using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MicroFlow.Meta
{
  internal sealed class SchemeDeclarator : INodeVisitor
  {
    private readonly List<StatementSyntax> myStatements;
    private readonly IVariableManager myVariableManager;
    private readonly IdentifierNameSyntax myBuilder;

    public SchemeDeclarator(
      [NotNull] List<StatementSyntax> statements,
      [NotNull] IVariableManager variableManager,
      [NotNull] IdentifierNameSyntax builder)
    {
      myStatements = statements.NotNull();
      myVariableManager = variableManager.NotNull();
      myBuilder = builder.NotNull();
    }

    public void AddDeclarations([NotNull] List<NodeInfo> nodes, [NotNull] List<VariableInfo> variables)
    {
      foreach (var variable in variables)
      {
        AddDeclaration(variable);
      }

      foreach (var node in nodes)
      {
        node.Accept(this);
      }

      var connector = new NodeConnector(myStatements, myVariableManager);
      connector.Connect(nodes);

      var binder = new Binder(myStatements, myVariableManager);
      binder.Bind(nodes);
    }

    public void Visit(ActivityInfo node)
    {
      AddDeclaration(
        node,
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            myBuilder,
            GenericName(
              Identifier(node.IsFaultHandler ? "FaultHandler" : "Activity"))
              .WithTypeArgumentList(
                TypeArgumentList(
                  SingletonSeparatedList(
                    node.ActivityType.ToTypeSyntax())))))
          .WithArgumentList(
            GetDescriptionArgumentList(node.Description)));

      if (node.Result != null)
      {
        var resultStatement = CreateDeclaration(
          node.Result.Name,
          InvocationExpression(
            MemberAccessExpression(
              SyntaxKind.SimpleMemberAccessExpression,
              GenericName(
                Identifier("Result"))
                .WithTypeArgumentList(
                  TypeArgumentList(
                    SingletonSeparatedList(
                      node.Result.Type.ToTypeSyntax()))),
              IdentifierName("Of")))
            .WithArgumentList(
              ArgumentList(
                SingletonSeparatedList(
                  Argument(
                    IdentifierName(myVariableManager.GetVariableOf(node)))))));

        myStatements.Add(resultStatement);
      }
    }

    public void Visit(ConditionInfo node)
    {
      AddDeclaration(
        node,
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            myBuilder,
            IdentifierName("Condition")))
          .WithArgumentList(
            GetDescriptionArgumentList(node.Description)));
    }

    public void Visit(SwitchInfo node)
    {
      AddDeclaration(
        node,
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            myBuilder,
            GenericName(
              Identifier("SwitchOf"))
              .WithTypeArgumentList(
                TypeArgumentList(
                  SingletonSeparatedList(
                    node.Type.ToTypeSyntax())))))
          .WithArgumentList(
            GetDescriptionArgumentList(node.Description)));
    }

    public void Visit(ForkJoinInfo node)
    {
      AddDeclaration(
        node,
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            myBuilder,
            IdentifierName("ForkJoin")))
          .WithArgumentList(
            GetDescriptionArgumentList(node.Description)));

      var forkJoinVariable = myVariableManager.GetVariableOf(node);

      var identifier = IdentifierName(forkJoinVariable);

      foreach (var fork in node.Forks)
      {
        AddDeclaration(
          fork,
          InvocationExpression(
            MemberAccessExpression(
              SyntaxKind.SimpleMemberAccessExpression,
              identifier,
              GenericName(
                Identifier("Fork"))
                .WithTypeArgumentList(
                  TypeArgumentList(
                    SingletonSeparatedList(
                      fork.ActivityType.ToTypeSyntax())))))
            .WithArgumentList(
              GetDescriptionArgumentList(fork.Description)));
      }
    }

    public void Visit(BlockInfo node)
    {
      var blockVariable = myVariableManager.GetVariableName();
      myVariableManager.AddVariable(node, blockVariable);

      var outerBuilder = myVariableManager.GetVariableName();
      var nestedBuilder = myVariableManager.GetVariableName();

      var nestedStatements = new List<StatementSyntax>();

      var nestedDeclarator = new SchemeDeclarator(
        nestedStatements,
        myVariableManager,
        IdentifierName(nestedBuilder));

      nestedDeclarator.AddDeclarations(node.Nodes, node.Variables);

      var declaration = CreateBlock(
        node.Description, 
        blockVariable, 
        outerBuilder, nestedBuilder, 
        nestedStatements);

      myStatements.Add(declaration);
    }

    private void AddDeclaration(NodeInfo node, InvocationExpressionSyntax invocation)
    {
      string variableName = myVariableManager.GetVariableName();
      myVariableManager.AddVariable(node, variableName);

      var statement = CreateDeclaration(variableName, invocation);

      myStatements.Add(statement);
    }

    private void AddDeclaration(VariableInfo variable)
    {
      ArgumentListSyntax argumentList;

      if (variable.InitialValueExpression != null)
      {
        argumentList = ArgumentList(
          SingletonSeparatedList(
            Argument(
              variable.InitialValueExpression.ToExpression())));
      }
      else
      {
        argumentList = ArgumentList();
      }

      var declaration = CreateDeclaration(
        variable.Name,
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            myBuilder,
            GenericName(
              Identifier("Variable"))
              .WithTypeArgumentList(
                TypeArgumentList(
                  SingletonSeparatedList(
                    variable.Type.ToTypeSyntax())))))
          .WithArgumentList(argumentList));

      myStatements.Add(declaration);
    }

    private static LocalDeclarationStatementSyntax CreateDeclaration(
      string variableName, InvocationExpressionSyntax invocation)
    {
      return LocalDeclarationStatement(
        VariableDeclaration(
          IdentifierName("var"))
          .WithVariables(
            SingletonSeparatedList(
              VariableDeclarator(
                Identifier(variableName))
                .WithInitializer(
                  EqualsValueClause(invocation)))));
    }

    private static ArgumentListSyntax GetDescriptionArgumentList(string name)
    {
      if (name == null) return ArgumentList();

      return ArgumentList(
        SingletonSeparatedList(
          Argument(
            LiteralExpression(
              SyntaxKind.StringLiteralExpression,
              Literal(name)))));
    }

    private LocalDeclarationStatementSyntax CreateBlock(
      string description, string blockVariable,
      string outerBuilder, string nestedBuilder,
      List<StatementSyntax> nestedStatements)
    {
      return CreateDeclaration(
        blockVariable,
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            myBuilder,
            IdentifierName("Block")))
          .WithArgumentList(
            ArgumentList(
              SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                  Argument(
                    LiteralExpression(
                      SyntaxKind.StringLiteralExpression,
                      Literal(description ?? "_"))),
                  Token(SyntaxKind.CommaToken),
                  Argument(
                    ParenthesizedLambdaExpression(
                      Block(nestedStatements))
                      .WithParameterList(
                        ParameterList(
                          SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                              Parameter(
                                Identifier(outerBuilder)),
                              Token(SyntaxKind.CommaToken),
                              Parameter(
                                Identifier(nestedBuilder))
                            }))))
                }))));
    }
  }
}