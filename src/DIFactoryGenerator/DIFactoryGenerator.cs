using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DIFactoryGenerator
{
	[Generator]
	public class DIFactoryGenerator : ISourceGenerator
	{
		public void Execute(GeneratorExecutionContext context)
		{
			if (!(context.SyntaxReceiver is SyntaxAnalyzer analyzer))
				return;

			foreach(ClassDeclarationSyntax item in analyzer.PossibleClasses)
			{
				ProcessClass(context, item);

			}

			string code = $@"
			namespace DIFactoryGenerator.Factories 
			{{
				public static class qwert
				{{
		
				}}
			}}";

			context.AddSource("qw", code);
		}

		private void ProcessClass(GeneratorExecutionContext context , ClassDeclarationSyntax syntax )
		{
			var model = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
			var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;

			if(symbol is null)
				return;

			if (HasFactory(symbol))
			{
				GenerateFactory(context, symbol);
			}

		}

		private bool HasFactory(INamedTypeSymbol symbol)
		{
			foreach(var item in symbol.GetAttributes())
			{
				var name = item.AttributeClass.ToDisplayString();
				if(name == "AEInject.Lib.Attribute.DIFactoryAttribute")
				{
					return true;
				}
			}
			return false;
		}

		private DIFactoryInfo GetFactoryInfo(INamedTypeSymbol symbol)
		{
			var factoryAtrr = symbol.GetAttributes()
				.FirstOrDefault(c =>
				c.AttributeClass.ToDisplayString() == "AEInject.Lib.Attribute.DIFactoryAttribute");

			if (factoryAtrr == null)
				return null ;

			var info = new DIFactoryInfo();

			if(factoryAtrr.ConstructorArguments.Length > 0)
			{
				foreach (var item in factoryAtrr.ConstructorArguments)
				{
					if(item.Kind == TypedConstantKind.Primitive && item.Value is string value )
						info.Name = value;
					
				}
			}

			
			foreach(var item in factoryAtrr.NamedArguments)
			{
				if(item.Key == "FactoryName" && item.Value.Value is string value)
				{
					info.Name = value;
				}
			}

			if (string.IsNullOrEmpty(info.Name))
			{
				info.Name = $"{symbol.Name}_DIFactory";
			}

			return info;

		}

		



		private void GenerateFactory(GeneratorExecutionContext context , INamedTypeSymbol symbol)
		{
			DIFactoryInfo dIFactoryInfo = GetFactoryInfo(symbol);


			string code = $@"
			namespace DIFactoryGenerator.Factories 
			{{
				public static class {dIFactoryInfo.Name} 
				{{
		
				}}
			}}";

			context.AddSource(dIFactoryInfo.Name, code);
		}


		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new SyntaxAnalyzer());
		}
	}
}
