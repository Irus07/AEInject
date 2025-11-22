using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastAccessor.Extensions;
using Microsoft.CodeAnalysis;
using static FastAccessor.Extensions.AccessExpander;

namespace DIFactoryGenerator.Builders
{
	internal static class CodeBuilder
	{
		// const AttributeData INJECT_ATRIBUTE_DATA = ;
		const string NAME_POSTFIX = DIFactoryGenerator.NAME_POSTFIX;


		internal static void GenerateSourceCode(SourceProductionContext context, ISymbol symbol)
		{

			if (!(symbol is INamedTypeSymbol typeSymbol))
			{
				return;
			}

			// Debugger.Launch();


			string code = $@"
using AEInject.Lib.DI.Services;
using static FastAccessor.Extensions.AccessExpander;
using AEinject.Lib.Attribute;
namespace DIFactoryGenerator.Factories 
{{
	[LocatorFactoryAttribute(typeof({typeSymbol.ToDisplayString()}),
							 typeof({symbol.Name + "_IncrementalFactory"}))]
	internal static class {symbol.Name + "_IncrementalFactory"} 
	{{
{GetConstructors(symbol)}
{GetFieldInjectAttr(typeSymbol).GetAwaiter().GetResult()}


	}}
}}";

			context.AddSource(symbol.Name + NAME_POSTFIX + ".g", code);

		}
		private static string GetConstructors(ISymbol symbol)
		{
			if (!(symbol is INamedTypeSymbol typeSymbol))
			{
				return "";
			}

			string className = typeSymbol.Name;
			var constructors = typeSymbol.Constructors;

			StringBuilder sb = new StringBuilder();



			foreach (var constructor in constructors)
			{
				MethodBuilder mb = new MethodBuilder("internal", true, typeSymbol.ToDisplayString(), "Create", constructor);

				mb.Body.AppendLine($"var instance = new {typeSymbol.ToDisplayString()}({mb.GetParamsNameString()});");
				mb.Body.AppendLine($"SetDependencies(instance);");
				mb.Body.AppendLine($"return instance ;");

				sb.AppendLine("\t\t" + mb.ToString());
			}



			return sb.ToString();
		}

		private static async Task<string> GetFieldInjectAttr(INamedTypeSymbol symbol)
		{
			var rez = await Task<IEnumerable<ISymbol>>.Run(() =>
			 {
				 var sortrez =
					 from x in symbol.GetMembers()
					 where x.GetAttributes().FirstOrDefault(y => y.AttributeClass.ToDisplayString() == "AEInject.Lib.Attribute.InjectAttribute") != null
					 select x;
				 return sortrez;
			 });

			MethodBuilder mb = new MethodBuilder("internal", true, "void", "SetDependencies", (symbol.ToDisplayString(), "obj"));

			foreach (var x in rez)
			{
				if (x is IFieldSymbol field)
				{
					mb.Body.AppendLine($"obj.SetValueField(\"{x.Name}\", ServiceProvider.GetServiceInstance<{field.Type}>());");
				}

			}
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("\t\t" + mb.ToString());
			return sb.ToString();
		}

	}
}
