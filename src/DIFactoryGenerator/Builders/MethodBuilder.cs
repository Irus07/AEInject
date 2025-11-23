using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace DIFactoryGenerator.Builders
{
	class MethodBuilder
	{
		private readonly string _accessModifier;
		private readonly string _name;
		private readonly string _type;
		private readonly bool _isStatic;
		private readonly List<(string type, string paramName)> _params;
		private StringBuilder _body = new StringBuilder();

		public StringBuilder Body
		{
			get => _body;
			set => _body = value;
		}

		public MethodBuilder(string accessModifier, bool isStatic, string returnType, string name, IEnumerable<(string type, string paramName)> @params = null)
		{
			_accessModifier = accessModifier;
			_isStatic = isStatic;
			_type = returnType;
			_name = name;
			if (@params is null)
			{
				_params = new List<(string type, string paramName)>();
			}
			else
			{
				_params = @params.ToList();
			}


		}
		public MethodBuilder(string accessModifier, bool isStatic, string returnType, string name, params (string type, string paramName)[] @params)
		{
			_accessModifier = accessModifier;
			_isStatic = isStatic;
			_type = returnType;
			_name = name;
			_params = @params.ToList();
		}
		public MethodBuilder(string accessModifier, bool isStatic, string returnType, string name, IMethodSymbol @params)
		{
			_accessModifier = accessModifier;
			_isStatic = isStatic;
			_type = returnType;
			_name = name;
			_params = new List<(string type, string paramName)>();

			string paramType, paramName;

			foreach (var symbol in @params.Parameters)
			{
				paramType = symbol.Type.ToDisplayString();
				paramName = symbol.Name;

				_params.Add((paramType, paramName));
			}

		}
		public MethodBuilder(string returnType, string name, IEnumerable<(string type, string paramName)> @params = null, string accessModifier = "private", bool isStatic = false)
		{
			_accessModifier = accessModifier;
			_isStatic = isStatic;
			_type = returnType;
			_name = name;
			_params = @params.ToList();
		}
		public void AddParam(string paramType, string paramName, string defValue = null)
		{
			if (!(defValue is null))
			{
				paramName += $" = {defValue}";
				_params.Insert(_params.Count, (paramType, paramName));
			}
			else
			{
				_params.Add((paramType, paramName));

			}


		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			string @static = _isStatic ? "static" : string.Empty;

			sb.AppendLine($"{_accessModifier} {@static} {_type} {_name} ({ParamsToString()})");
			sb.AppendLine("{");

			foreach (var str in Body.ToString().Split('\n'))
			{
				sb.AppendLine("\t" + str);
			}

			sb.AppendLine("}");

			return sb.ToString();
		}

		private string ParamsToString()
		{
			StringBuilder sb = new StringBuilder();
			if (_params.Count == 0)
				return string.Empty;


			for (int i = 0; i < _params.Count; i++)
			{
				if (i < _params.Count - 1)
					sb.Append($"{_params[i].type} {_params[i].paramName}, ");
				else
					sb.Append($"{_params[i].type} {_params[i].paramName}");
			}

			return sb.ToString();
		}
		public string GetParamsNameString()
		{
			StringBuilder sb = new StringBuilder();
			if (_params.Count == 0)
				return string.Empty;


			for (int i = 0; i < _params.Count; i++)
			{
				if (i < _params.Count - 1)
					sb.Append($"{_params[i].paramName}, ");
				else
					sb.Append($"{_params[i].paramName}");
			}

			return sb.ToString();
		}
		public IEnumerable<(string paramType, string paramName)> EnumerateParams() => _params;

	}
}