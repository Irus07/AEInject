using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection.Metadata;

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
					(s, _) => (s is ClassDeclarationSyntax),
					(ctx, _) =>
					{
						var classSyntax = (ClassDeclarationSyntax)ctx.Node;
						return ctx.SemanticModel.GetDeclaredSymbol(classSyntax);
					})
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



	}
}
