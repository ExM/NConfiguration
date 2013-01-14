using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Configuration.GenericView.Deserialization
{
	public class ComplexFunctionBuilder
	{
		private Type _targetType;
		private IGenericDeserializer _deserializer;
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;

		public ComplexFunctionBuilder(Type targetType, IGenericDeserializer deserializer)
		{
			_targetType = targetType;
			_pResult = Expression.Parameter(_targetType);
			_deserializer = deserializer;
		}

		public event EventHandler<FieldFunctionBuildingEventArgs> FieldFunctionBuilding;

		private FieldFunctionBuildingEventArgs OnFieldFunctionBuilding(Type type, string name, object[] customAttributes)
		{
			var args = new FieldFunctionBuildingEventArgs(type, name, customAttributes);
			var copy = FieldFunctionBuilding;
			if (copy != null)
				copy(this, args);

			return args;
		}

		public Type TargetType
		{
			get
			{
				return _targetType;
			}
		}

		public Expression Deserializer
		{
			get
			{
				return Expression.Constant(_deserializer);
			}
		}

		public Expression CfgNode
		{
			get
			{
				return _pCfgNode;
			}
		}

		private void SetConstructor()
		{
			if (_targetType.IsValueType)
				return;
			
			var ci = _targetType.GetConstructor(new Type[] { });
			if (ci == null)
				throw new InvalidOperationException("default constructor not found");

			_bodyList.Add(Expression.Assign(_pResult, Expression.New(ci)));
		}

		public object Compile()
		{
			try
			{
				SetConstructor();

				foreach (var fi in _targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					var right = CreateFunction(fi.FieldType, fi.Name, fi.GetCustomAttributes(true));
					if (right == null)
						continue;
					var left = Expression.Field(_pResult, fi);
					_bodyList.Add(Expression.Assign(left, right));
				}

				foreach (var pi in _targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					if (!pi.CanWrite)
						continue;
					var right = CreateFunction(pi.PropertyType, pi.Name, pi.GetCustomAttributes(true));
					if (right == null)
						continue;
					var left = Expression.Property(_pResult, pi);
					_bodyList.Add(Expression.Assign(left, right));
				}

				_bodyList.Add(Expression.Label(Expression.Label(_targetType), _pResult));

				var delegateType = typeof(Func<,>).MakeGenericType(typeof(ICfgNode), _targetType);

				return Expression.Lambda(delegateType, Expression.Block(new[] { _pResult }, _bodyList), _pCfgNode).Compile();
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("can't create a deserialize function for '{0}'", _targetType.FullName), ex);
			}
		}

		private Expression CreateFunction(Type fieldType, string fieldName, object[] customAttributes)
		{
			var args = OnFieldFunctionBuilding(fieldType, fieldName, customAttributes);
			if (args.Ignore)
				return null;

			return MakeFieldReader(args);
		}

		private Expression MakeFieldReader(FieldFunctionBuildingEventArgs args)
		{
			if (args.Type.IsArray)
			{
				var itemType = args.Type.GetElementType();
				var mi = typeof(BuildToolkit).GetMethod("Array").MakeGenericMethod(itemType);
				return Expression.Call(null, mi, Expression.Constant(args.Name), CfgNode, Deserializer);
			}

			if (args.Function == FieldFunctionType.Primitive)
			{
				var methodName = args.Required ? "RequiredPrimitiveField" : "OptionalPrimitiveField";
				var mi = typeof(BuildToolkit).GetMethod(methodName).MakeGenericMethod(args.Type);
				return Expression.Call(null, mi, Expression.Constant(args.Name), CfgNode);
			}

			if (args.Function == FieldFunctionType.Collection)
			{
				var itemType = args.Type.GetGenericArguments()[0];
				var mi = typeof(BuildToolkit).GetMethod("List").MakeGenericMethod(itemType);
				return Expression.Call(null, mi, Expression.Constant(args.Name), CfgNode, Deserializer);
			}

			if (args.Function == FieldFunctionType.Complex)
			{
				var methodName = args.Required ? "RequiredComplexField" : "OptionalComplexField";
				var mi = typeof(BuildToolkit).GetMethod(methodName).MakeGenericMethod(args.Type);
				return Expression.Call(null, mi, Expression.Constant(args.Name), CfgNode, Deserializer);
			}

			throw new InvalidOperationException("unexpected FieldFunctionType");
		}
	}
}
