using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection.Metadata;
using System.Threading;
using DIFactoryGenerator.Builders;

namespace DIFactoryGenerator
{
    [Generator]
	public class DIFactoryGenerator : IIncrementalGenerator
	{
		public const string ATTR_NAME = "AEInject.Lib.Attribute.DIFactoryAttribute";
		public const string NAME_POSTFIX = "_IncrementalFactory";


		public void Initialize(IncrementalGeneratorInitializationContext context)
		{

			var classProvider = context.SyntaxProvider
				.CreateSyntaxProvider(
					NodeTemplate,
					SyntaxTemplate
					)
				.Where(rez => !(rez is null));



			var filteredClasses =
				from x in classProvider
				where x.GetAttributes()
					.Any(atr => atr.AttributeClass.ToDisplayString() == ATTR_NAME)
				select x;


			context.RegisterSourceOutput(filteredClasses, (productionContext, classSymbol) =>
			{
				CodeBuilder.GenerateSourceCode(productionContext, classSymbol);
			});

		}

		private static bool NodeTemplate(SyntaxNode node, CancellationToken token) => node is ClassDeclarationSyntax;

		private static ISymbol SyntaxTemplate(GeneratorSyntaxContext context, CancellationToken token)
		{
			var classSyntax = (ClassDeclarationSyntax)context.Node;
			return context.SemanticModel.GetDeclaredSymbol(classSyntax);
		}

	}
}
