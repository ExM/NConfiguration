using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Configuration.GenericView.Deserialization
{
	public class MapFunctionBuilder
	{
		private Type _targetType;
		private IGenericDeserializer _deserializer;
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;
		private FieldReaderCreator _frCreator;

		public MapFunctionBuilder(Type targetType, IGenericDeserializer deserializer, FieldReaderCreator frCreator)
		{
			_targetType = targetType;
			_pResult = Expression.Parameter(_targetType);
			_deserializer = deserializer;
			_frCreator = frCreator;

			SetConstructor();
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
				throw new NotImplementedException("default constructor not found");

			_bodyList.Add(Expression.Assign(_pResult, Expression.New(ci)));
		}

		public object Compile()
		{
			foreach (var fi in _targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				var right = CreateLoader(fi.FieldType, fi.Name, fi.GetCustomAttributes(true));
				if (right == null)
					continue;
				var left = Expression.Field(_pResult, fi);
				_bodyList.Add(Expression.Assign(left, right));
			}

			foreach (var pi in _targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (!pi.CanWrite)
					continue;
				var right = CreateLoader(pi.PropertyType, pi.Name, pi.GetCustomAttributes(true));
				if (right == null)
					continue;
				var left = Expression.Property(_pResult, pi);
				_bodyList.Add(Expression.Assign(left, right));
			}

			_bodyList.Add(Expression.Label(Expression.Label(_targetType), _pResult));

			var delegateType = typeof (Func<,>).MakeGenericType(typeof (ICfgNode), _targetType);

			return Expression.Lambda(delegateType, Expression.Block(new[] { _pResult }, _bodyList), _pCfgNode).Compile();
		}

		private Expression CreateLoader(Type fieldType, string name, object[] customAttributes)
		{
			return _frCreator(this, fieldType, name, customAttributes);
		}
	}
}
