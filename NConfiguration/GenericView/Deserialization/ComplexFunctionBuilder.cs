using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace NConfiguration.GenericView.Deserialization
{
	public class ComplexFunctionBuilder
	{
		private Type _targetType;
		private IGenericDeserializer _deserializer;
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;
		private Action<FieldFunctionInfo, object[]> _configureFieldInfo;

		public ComplexFunctionBuilder(Type targetType, IGenericDeserializer deserializer, Action<FieldFunctionInfo, object[]> configureFieldInfo)
		{
			_targetType = targetType;
			_pResult = Expression.Parameter(_targetType);
			_deserializer = deserializer;
			_configureFieldInfo = configureFieldInfo;
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
			var ffi = new FieldFunctionInfo(fieldType, fieldName);
			_configureFieldInfo(ffi, customAttributes);
			return ffi.Ignore ? null : MakeFieldReader(ffi);
		}

		private Expression MakeFieldReader(FieldFunctionInfo ffi)
		{
			switch (ffi.Function)
			{
				case FieldFunctionType.Primitive:
				{
					var mi = ffi.Required ? BuildToolkit.RequiredPrimitiveFieldMI : BuildToolkit.OptionalPrimitiveFieldMI;
					mi = mi.MakeGenericMethod(ffi.ResultType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode);
				}

				case FieldFunctionType.Array:
				{
					var itemType = ffi.ResultType.GetElementType();
					var mi = BuildToolkit.ArrayMI.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(_deserializer));
				};

				case FieldFunctionType.Collection:
				{
					var itemType = ffi.ResultType.GetGenericArguments()[0];
					var mi = BuildToolkit.ListMI.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(_deserializer));
				};

				case FieldFunctionType.Complex:
				{
					var mi = ffi.Required ? BuildToolkit.RequiredComplexFieldMI : BuildToolkit.OptionalComplexFieldMI;
					mi = mi.MakeGenericMethod(ffi.ResultType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(_deserializer));
				};

				default:
					throw new InvalidOperationException("unexpected FieldFunctionType");
			}
		}
	}
}
