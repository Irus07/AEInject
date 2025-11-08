using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace DIFactoryGenerator.Builders
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
                MethodBuilder mb = new MethodBuilder("public", true, "void", "CreateFromMethodBuilder", constructor);
                sb.AppendLine(mb.ToString());
            }

            return sb.ToString();
        }
    }
}
