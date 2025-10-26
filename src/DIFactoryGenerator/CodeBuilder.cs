using System.Text;
using Microsoft.CodeAnalysis;

namespace DIFactoryGenerator
{
	internal static class CodeBuilder
	{
		const string NAME_POSTFIX = DIFactoryGenerator.NAME_POSTFIX;


		internal static void GenerateSourceCode(SourceProductionContext context, ISymbol symbol)
		{

			//Debugger.Launch();


			string code = $@"
				namespace DIFactoryGenerator.Factories 
							{{
								public static class {symbol.Name + "_IncrementalFactory"} 
								{{
									{GetConstructors(symbol)}
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
				//sb.Append($"public static {symbol.Name}  Create(");
				sb.Append($"public static void  Create(");
				int leng = constructor.Parameters.Length, counter = 0;
				foreach (var parameter in constructor.Parameters)
				{
					string paramType = parameter.Type.ToDisplayString();
					string paramName = parameter.Name;
					sb.Append(paramType + $" {paramName}");
					if (counter < leng - 1)
					{
						sb.Append(", ");
					}
					counter++;

				}


				//sb.Remove(sb.Length - 1, 1);
				sb.Append(")");
				sb.AppendLine("{}");
			}

			return sb.ToString();
		}
	}
}
