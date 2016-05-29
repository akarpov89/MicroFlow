using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MicroFlow.Meta
{
  internal static class SyntaxUtil
  {
    private static readonly char[] NamespaceSeparator = { '.' };

    private static readonly Dictionary<Type, SyntaxKind> PredefinedTypes = new Dictionary<Type, SyntaxKind>
    {
      [typeof(bool)] = SyntaxKind.BoolKeyword,
      [typeof(char)] = SyntaxKind.CharKeyword,
      [typeof(sbyte)] = SyntaxKind.SByteKeyword,
      [typeof(byte)] = SyntaxKind.ByteKeyword,
      [typeof(short)] = SyntaxKind.ShortKeyword,
      [typeof(ushort)] = SyntaxKind.UShortKeyword,
      [typeof(int)] = SyntaxKind.IntKeyword,
      [typeof(uint)] = SyntaxKind.UIntKeyword,
      [typeof(long)] = SyntaxKind.LongKeyword,
      [typeof(ulong)] = SyntaxKind.ULongKeyword,
      [typeof(float)] = SyntaxKind.FloatKeyword,
      [typeof(double)] = SyntaxKind.DoubleKeyword,
      [typeof(decimal)] = SyntaxKind.DecimalKeyword,
      [typeof(string)] = SyntaxKind.StringKeyword,
      [typeof(object)] = SyntaxKind.ObjectKeyword
    };

    public static ExpressionSyntax ToExpression(this string expression)
    {
      var syntaxTree = CSharpSyntaxTree.ParseText("class A { void M() => " + expression + "; }");
      var arrowExpressionClause = (ArrowExpressionClauseSyntax)syntaxTree
        .GetRoot()
        .DescendantNodes()
        .First(n => n.IsKind(SyntaxKind.ArrowExpressionClause));

      return arrowExpressionClause.Expression;
    }

    [NotNull, Pure]
    public static TypeSyntax ToTypeSyntax(this Type type)
    {
      SyntaxKind keyword;
      if (PredefinedTypes.TryGetValue(type, out keyword))
      {
        return keyword.ToPredefinedType();
      }

      if (type.IsArray)
      {
        Type elementType;
        int rank;
        GetElementAndRank(type, out elementType, out rank);

        var elementName = ToTypeSyntax(elementType);
        return GetArrayType(elementName, rank);
      }

      if (type.GetTypeInfo().IsGenericType)
      {
        var untickedName = type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal));
        var identifier = Identifier(untickedName);
        var typeArguments = GetTypeArguments(type.GetTypeInfo().GenericTypeArguments);
        var genericName = GenericName(identifier, typeArguments);

        return QualifiedName(GetNamespace(type), genericName);
      }

      if (type.IsPointer)
      {
        var dereferenceType = type.GetElementType();

        return PointerType(dereferenceType.ToTypeSyntax());
      }

      var ns = GetNamespace(type);
      var name = IdentifierName(type.Name);

      return ns == null ? (TypeSyntax) name : QualifiedName(ns, name);
    }

    [NotNull, Pure]
    private static PredefinedTypeSyntax ToPredefinedType(this SyntaxKind keyword)
    {
      return PredefinedType(Token(keyword));
    }

    [CanBeNull, Pure]
    private static NameSyntax GetNamespace([NotNull] Type type)
    {
      return type.Namespace?.ToNamespaceSyntax();
    }

    public static NameSyntax ToNamespaceSyntax(this string ns)
    {
      if (ns.IndexOf(".", StringComparison.Ordinal) == -1)
      {
        return IdentifierName(ns);
      }

      var namespaceParts = ns.Split(NamespaceSeparator, StringSplitOptions.RemoveEmptyEntries);

      var qualifiedName = QualifiedName(IdentifierName(namespaceParts[0]), IdentifierName(namespaceParts[1]));

      for (int i = 2; i < namespaceParts.Length; ++i)
      {
        qualifiedName = QualifiedName(qualifiedName, IdentifierName(namespaceParts[i]));
      }

      return qualifiedName;
    }

    [NotNull, Pure]
    private static ArrayTypeSyntax GetArrayType([NotNull] TypeSyntax elementType, int rank)
    {
      var arrayTypeSyntax = ArrayType(elementType);

      var ranks = new ArrayRankSpecifierSyntax[rank];
      for (int i = 0; i < rank; ++i)
      {
        ranks[i] = CreateArrayRank();
      }

      return arrayTypeSyntax.WithRankSpecifiers(List(ranks));
    }

    [NotNull, Pure]
    private static ArrayRankSpecifierSyntax CreateArrayRank()
    {
      return ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()));
    }

    [NotNull, Pure]
    private static TypeArgumentListSyntax GetTypeArguments([NotNull] Type[] arguments)
    {
      var typeList = new TypeSyntax[arguments.Length];
      for (int i = 0; i < arguments.Length; ++i)
      {
        typeList[i] = ToTypeSyntax(arguments[i]);
      }

      return TypeArgumentList(SeparatedList(typeList));
    }

    private static void GetElementAndRank([NotNull] Type arrayType, [NotNull] out Type elementType, out int rank)
    {
      rank = arrayType.GetArrayRank();

      elementType = arrayType;

      for (int i = 0; i < rank; ++i)
      {
        elementType = elementType.GetElementType();
      }
    }
  }
}