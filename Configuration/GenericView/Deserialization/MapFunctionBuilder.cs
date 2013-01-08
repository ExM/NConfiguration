using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Configuration.GenericView.Deserialization
{
	internal class MapFunctionBuilder<T>
	{
		private Type _resultType = typeof(T);
		private GenericMapper _mapper;
		private IGenericDeserializer _deserializer;
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;

		public MapFunctionBuilder(GenericMapper mapper, IGenericDeserializer deserializer)
		{
			_pResult = Expression.Parameter(_resultType);
			_mapper = mapper;
			_deserializer = deserializer;

			SetConstructor();
		}

		private void SetConstructor()
		{
			if (_resultType.IsValueType)
				return;
			
			var ci = _resultType.GetConstructor(new Type[] { });
			if (ci == null)
				throw new NotImplementedException("default constructor not found");

			_bodyList.Add(Expression.Assign(_pResult, Expression.New(ci)));
		}

		public Func<ICfgNode, T> Compile()
		{
			foreach (var fi in _resultType.GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				var right = CreateLoader(fi.FieldType, fi.Name, fi.GetCustomAttributes(true));
				if (right == null)
					continue;
				var left = Expression.Field(_pResult, fi);
				_bodyList.Add(Expression.Assign(left, right));
			}

			foreach (var pi in _resultType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (!pi.CanWrite)
					continue;
				var right = CreateLoader(pi.PropertyType, pi.Name, pi.GetCustomAttributes(true));
				if (right == null)
					continue;
				var left = Expression.Property(_pResult, pi);
				_bodyList.Add(Expression.Assign(left, right));
			}

			_bodyList.Add(Expression.Label(Expression.Label(_resultType), _pResult));

			Func<ICfgNode, T> loader = Expression
				.Lambda<Func<ICfgNode, T>>(Expression.Block(new[] { _pResult }, _bodyList), _pCfgNode)
				.Compile();

			return loader;
		}

		private Expression CreateLoader(Type agrType, string name, object[] customAttributes)
		{
			var right = Expression.Call(null, typeof(LoadToolkit).GetMethod("RequiredField").MakeGenericMethod(agrType), _pCfgNode, Expression.Constant(name));

			return right;
		}
	}
}
