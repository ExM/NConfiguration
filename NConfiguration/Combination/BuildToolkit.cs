using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Reflection;

namespace NConfiguration.Combination
{
	public static class BuildToolkit
	{
		internal static bool IsNullable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		internal static object CreateRecursiveNullableCombiner(Type type, IGenericCombiner combiner)
		{
			var funcType = typeof(Func<,,>).MakeGenericType(type, type, type);

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype == null) // is Nullable<>
				throw new ArgumentOutOfRangeException("type", "must be a nullable");

			// this is currying. Reduce first argument in RecursiveNullableCombine
			return Delegate.CreateDelegate(funcType, combiner, RecursiveNullableCombineMI.MakeGenericMethod(ntype));
		}

		internal static readonly MethodInfo RecursiveNullableCombineMI = typeof(BuildToolkit).GetMethod("RecursiveNullableCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T? RecursiveNullableCombine<T>(IGenericCombiner combiner, T? x, T? y) where T : struct
		{
			if (!x.HasValue)
				return y;

			if (!y.HasValue)
				return x;

			var result = combiner.Combine<T>(x.Value, y.Value);

			return result;
		}

		internal static object CreateForwardCombiner(Type type)
		{
			var funcType = typeof(Func<,,>).MakeGenericType(type, type, type);

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype != null) // is Nullable<>
				return Delegate.CreateDelegate(funcType, ForwardNullableStructCombineMI.MakeGenericMethod(ntype));

			if(type.IsValueType)
				return Delegate.CreateDelegate(funcType, SelectStructCombiner(type).MakeGenericMethod(type));
			else
				return Delegate.CreateDelegate(funcType, ForwardClassCombineMI.MakeGenericMethod(type));
		}

		internal static MethodInfo SelectStructCombiner(Type type)
		{
			var eqInterface = typeof(IEquatable<>).MakeGenericType(type);
			if (eqInterface.IsAssignableFrom(type))
				return ForwardEquatableStructCombineMI;

			var comInterface = typeof(IComparable<>).MakeGenericType(type);
			if (comInterface.IsAssignableFrom(type))
				return ForwardComparableStructCombineMI;

			return ForwardOthersStructCombineMI;
		}

		internal static readonly MethodInfo ForwardClassCombineMI = typeof(BuildToolkit).GetMethod("ForwardClassCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T ForwardClassCombine<T>(T x, T y)  where T: class
		{
			return y == null ? x : y;
		}

		internal static readonly MethodInfo ForwardNullableStructCombineMI = typeof(BuildToolkit).GetMethod("ForwardNullableStructCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T? ForwardNullableStructCombine<T>(T? x, T? y) where T : struct
		{
			return y.HasValue ? y : x;
		}

		internal static readonly MethodInfo ForwardEquatableStructCombineMI = typeof(BuildToolkit).GetMethod("ForwardEquatableStructCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T ForwardEquatableStructCombine<T>(T x, T y) where T : struct, IEquatable<T>
		{
			return y.Equals(default(T)) ? x : y;
		}

		internal static readonly MethodInfo ForwardComparableStructCombineMI = typeof(BuildToolkit).GetMethod("ForwardComparableStructCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T ForwardComparableStructCombine<T>(T x, T y) where T : struct, IComparable<T>
		{
			return y.CompareTo(default(T)) == 0 ? x : y;
		}

		internal static readonly MethodInfo ForwardOthersStructCombineMI = typeof(BuildToolkit).GetMethod("ForwardOthersStructCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T ForwardOthersStructCombine<T>(T x, T y) where T : struct
		{
			return y.Equals(default(T)) ? x : y;
		}
	}
}
