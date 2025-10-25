using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DIFactoryGenerator
{
	public class SyntaxAnalyzer : ISyntaxReceiver
	{
		public List<ClassDeclarationSyntax> PossibleClasses = new List<ClassDeclarationSyntax>();
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if( syntaxNode is ClassDeclarationSyntax classDeclaration)
			{
				if(classDeclaration.AttributeLists.Count > 0) 
					PossibleClasses.Add(classDeclaration);
			}
		}
	}
}
