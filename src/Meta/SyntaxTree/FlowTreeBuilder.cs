using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MicroFlow.Meta
{
  public class FlowTreeBuilder : IVariableManager
  {
    private readonly FlowScheme myScheme;

    private int myVarCounter;
    private readonly Dictionary<NodeInfo, string> myNodesToName = new Dictionary<NodeInfo, string>();

    public FlowTreeBuilder([NotNull] FlowScheme scheme)
    {
      myScheme = scheme;
    }

    public string GetVariableName()
    {
      return $"_gen_{++myVarCounter}";
    }

    public void AddVariable(NodeInfo node, string variableName)
    {
      myNodesToName.Add(node, variableName);
    }

    public string GetVariableOf(NodeInfo node)
    {
      return myNodesToName[node];
    }

    public SyntaxTree Build()
    {
      //
      // Syntax Tree
      //

      var compilationUnit = CompilationUnit()
        .WithUsings(GetUsings())
        .WithMembers(
          SingletonList<MemberDeclarationSyntax>(
            GetFlowNamespace()
              .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                  GetFlowClassDeclaration()
                    .WithMembers(
                      List(
                        GetFlowProperties()
                        .Concat(GetBuildMethod())
                        .Concat(GetConfigureServicesMethod())
                        .Concat(GetConfigureValidationMethod())
                        .Concat(GetCreateFlowExecutionLoggerMethod())))))))
        .NormalizeWhitespace();

      return compilationUnit.SyntaxTree;
    }

    private SyntaxList<UsingDirectiveSyntax> GetUsings()
    {
      var usings = myScheme.Namespaces.Select(ns => UsingDirective(ns.ToNamespaceSyntax()));

      return List(usings);
    }

    private NamespaceDeclarationSyntax GetFlowNamespace()
    {
      string flowFullTypeName = myScheme.FlowFullTypeName;
      var ns = flowFullTypeName.Substring(0, flowFullTypeName.LastIndexOf(".", StringComparison.Ordinal));

      return NamespaceDeclaration(ns.ToNamespaceSyntax());
    }

    private ClassDeclarationSyntax GetFlowClassDeclaration()
    {
      string flowFullTypeName = myScheme.FlowFullTypeName;

      var className = flowFullTypeName.Substring(flowFullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1);

      var declaration = ClassDeclaration(className)
        .WithModifiers(GetPublicModifier());

      SimpleBaseTypeSyntax baseType;

      if (myScheme.ResultType == null)
      {
        baseType = SimpleBaseType(IdentifierName("Flow"));
      }
      else
      {
        baseType = SimpleBaseType(
          GenericName(
            Identifier("Flow"))
            .WithTypeArgumentList(
              TypeArgumentList(
                SingletonSeparatedList(
                  myScheme.ResultType.ToTypeSyntax()))));
      }

      return declaration
        .WithBaseList(
          BaseList(
            SingletonSeparatedList<BaseTypeSyntax>(
              baseType)));
    }

    private static SyntaxTokenList GetPublicModifier()
    {
      return TokenList(Token(SyntaxKind.PublicKeyword));
    }

    private static SyntaxTokenList GetPublicOverrideModifiers()
    {
      return TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword));
    }

    private static SyntaxTokenList GetProtectedOverrideModifiers()
    {
      return TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword));
    }

    [NotNull]
    private IEnumerable<MemberDeclarationSyntax> GetFlowProperties()
    {
      yield return PropertyDeclaration(
        PredefinedType(
          Token(SyntaxKind.StringKeyword)),
        Identifier("Name"))
        .WithModifiers(
          GetPublicOverrideModifiers())
        .WithExpressionBody(
          ArrowExpressionClause(
            LiteralExpression(
              SyntaxKind.StringLiteralExpression,
              Literal(myScheme.Description))))
        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

      if (myScheme.Properties.Count == 0) yield break;

      foreach (var propertyInfo in myScheme.Properties)
      {
        yield return PropertyDeclaration(
          propertyInfo.Type.ToTypeSyntax(),
          Identifier(propertyInfo.Name))
          .WithModifiers(GetPublicModifier())
          .WithAccessorList(
            AccessorList(
              List(
                new[]
                {
                  AccessorDeclaration(
                    SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(
                      Token(SyntaxKind.SemicolonToken)),
                  AccessorDeclaration(
                    SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(
                      Token(SyntaxKind.SemicolonToken))
                })));
      }
    }

    private IEnumerable<MemberDeclarationSyntax> GetBuildMethod()
    {
      yield return
        CreateMethod("Build", "FlowBuilder", "builder")
        .WithBody(
          Block(GetFlowBuildBody()));
    }

    private IEnumerable<MemberDeclarationSyntax> GetConfigureServicesMethod()
    {
      if (myScheme.Services.Count == 0) yield break;

      yield return
        CreateMethod("ConfigureServices", "IServiceCollection", "services")
        .WithBody(
          Block(GetConfigureServicesBody()));
    }

    private IEnumerable<MemberDeclarationSyntax> GetConfigureValidationMethod()
    {
      if (myScheme.Validators.Count == 0) yield break;

      yield return
        CreateMethod("ConfigureValidation", "IValidatorCollection", "validators")
        .WithBody(
          Block(GetConfigureValidationBody()));
    }

    private IEnumerable<MemberDeclarationSyntax> GetCreateFlowExecutionLoggerMethod()
    {
      if (myScheme.Logger == null) yield break;

      yield return
        MethodDeclaration(
          IdentifierName("ILogger"),
          Identifier("CreateFlowExecutionLogger"))
          .WithModifiers(
            GetProtectedOverrideModifiers())
          .WithBody(
            Block(GetCreateFlowExecutionLoggerBody()));
    }

    private MethodDeclarationSyntax CreateMethod(string methodName, string parameterType = null, string parameterName = null)
    {
      var method = MethodDeclaration(
        PredefinedType(
          Token(SyntaxKind.VoidKeyword)),
        Identifier(methodName))
        .WithModifiers(GetProtectedOverrideModifiers());

      if (parameterType == null || parameterName == null) return method;

      return method
        .WithParameterList(
          ParameterList(
            SingletonSeparatedList(
              Parameter(
                Identifier(parameterName))
                .WithType(
                  IdentifierName(parameterType)))));
    }

    private List<StatementSyntax> GetFlowBuildBody()
    {
      var builder = IdentifierName("builder");

      var statements = new List<StatementSyntax>();

      if (myScheme.DefaultFaultHandlerType != null)
      {
        statements.Add(
          AddDefaultXxxHandler(builder, "WithDefaultFaultHandler", myScheme.DefaultFaultHandlerType));
      }

      if (myScheme.DefaultCancellationHandlerType != null)
      {
        statements.Add(
          AddDefaultXxxHandler(builder, "WithDefaultCancellationHandler", myScheme.DefaultCancellationHandlerType));
      }

      var schemeDeclarator = new SchemeDeclarator(statements, this, builder);
      schemeDeclarator.AddDeclarations(myScheme.Nodes, myScheme.GlobalVariables);

      if (myScheme.IntialNode != null)
      {
        statements.Add(SetInitialNode(builder, IdentifierName(GetVariableOf(myScheme.IntialNode))));
      }

      return statements;
    }

    private static StatementSyntax AddDefaultXxxHandler(IdentifierNameSyntax builder, string method, Type handlerType)
    {
      return ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            builder,
            GenericName(
              Identifier(method))
              .WithTypeArgumentList(
                TypeArgumentList(
                  SingletonSeparatedList(
                    handlerType.ToTypeSyntax()))))));
    }

    private static StatementSyntax SetInitialNode(IdentifierNameSyntax builder, IdentifierNameSyntax initialNode)
    {
      return ExpressionStatement(
        InvocationExpression(
          MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            builder,
            IdentifierName("WithInitialNode")))
          .WithArgumentList(
            ArgumentList(
              SingletonSeparatedList(
                Argument(initialNode)))));
    }

    private List<StatementSyntax> GetConfigureServicesBody()
    {
      Debug.Assert(myScheme.Services != null);

      var statements = new List<StatementSyntax>();

      foreach (var service in myScheme.Services)
      {
        statements.Add(GetServiceRegistrationStatement(service));
      }

      return statements;
    }

    private StatementSyntax GetServiceRegistrationStatement(ServiceInfo serviceInfo)
    {
      string methodName;

      switch (serviceInfo.LifetimeKind)
      {
        case LifetimeKind.Transient:
          methodName = "AddTransient";
          break;
        case LifetimeKind.Singleton:
          methodName = "AddSingleton";
          break;
        case LifetimeKind.DisposableSingleton:
          methodName = "AddDisposableSingleton";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return
        ExpressionStatement(
          InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("services"),
            GenericName(Identifier(methodName))
            .WithTypeArgumentList(GetRegistrationTypeArguments(serviceInfo))))
            .WithArgumentList(GetRegistrationArguments(serviceInfo)));
    }

    private static TypeArgumentListSyntax GetRegistrationTypeArguments(ServiceInfo serviceInfo)
    {
      TypeArgumentListSyntax typeArguments;

      if (serviceInfo.Implementation == null)
      {
        typeArguments = TypeArgumentList(SingletonSeparatedList(serviceInfo.Interface.ToTypeSyntax()));
      }
      else
      {
        typeArguments = TypeArgumentList(SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]
        {
          serviceInfo.Interface.ToTypeSyntax(), Token(SyntaxKind.CommaToken), serviceInfo.Implementation.ToTypeSyntax()
        }));
      }
      return typeArguments;
    }

    private static ArgumentListSyntax GetRegistrationArguments(ServiceInfo serviceInfo)
    {
      ArgumentListSyntax arguments;

      if (serviceInfo.ConstructorArgumentExpressions != null && serviceInfo.ConstructorArgumentExpressions.Length > 0)
      {
        var ctorArgs = serviceInfo.ConstructorArgumentExpressions;

        var args = new SyntaxNodeOrToken[ctorArgs.Length*2 - 1];

        args[0] = Argument(ctorArgs[0].ToExpression());

        for (int i = 1; i < ctorArgs.Length; ++i)
        {
          args[2*i - 1] = Token(SyntaxKind.CommaToken);
          args[2*i] = Argument(ctorArgs[i].ToExpression());
        }

        arguments = ArgumentList(SeparatedList<ArgumentSyntax>(args));
      }
      else if (serviceInfo.InstanceExpression != null)
      {
        arguments = ArgumentList(SingletonSeparatedList(Argument(serviceInfo.InstanceExpression.ToExpression())));
      }
      else
      {
        arguments = ArgumentList();
      }
      return arguments;
    }

    private List<StatementSyntax> GetConfigureValidationBody()
    {
      Debug.Assert(myScheme.Validators != null);

      var statements = new List<StatementSyntax>();

      foreach (var validator in myScheme.Validators)
      {
        var statement =
          ExpressionStatement(
            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
              IdentifierName("validators"), IdentifierName("Add")))
              .WithArgumentList(
                ArgumentList(
                  SingletonSeparatedList(
                    Argument(ObjectCreationExpression(validator.ToTypeSyntax()).WithArgumentList(ArgumentList()))))));

        statements.Add(statement);
      }

      return statements;
    }

    private StatementSyntax GetCreateFlowExecutionLoggerBody()
    {
      Debug.Assert(myScheme.Logger != null);

      if (myScheme.Logger.Type != null)
      {
        return ReturnStatement(
          ObjectCreationExpression(
            myScheme.Logger.Type.ToTypeSyntax())
            .WithArgumentList(ArgumentList()));
      }

      return ReturnStatement(myScheme.Logger.Expression.ToExpression());
    }
  }
}