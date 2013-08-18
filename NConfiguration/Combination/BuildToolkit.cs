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

		internal static bool IsCollection(Type type)
		{
			if (type.IsArray)
				return true;

			if (!type.IsGenericType)
				return false;

			var genType = type.GetGenericTypeDefinition();

			return genType == typeof(List<>)
				|| genType == typeof(IList<>)
				|| genType == typeof(ICollection<>)
				|| genType == typeof(IEnumerable<>);
		}

		internal static object TryCreateRecursiveNullableCombiner(Type type, IGenericCombiner combiner)
		{
			var funcType = typeof(Func<,,>).MakeGenericType(type, type, type);

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype == null) // is not Nullable<>
				return null;

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

		internal static object TryCreateCollectionCombiner(Type type)
		{
			var funcType = typeof(Func<,,>).MakeGenericType(type, type, type);

			if (type.IsArray)
			{
				var itemType = type.GetElementType();
				return Delegate.CreateDelegate(funcType, ForwardArrayCombineMI.MakeGenericMethod(itemType));
			}


			if (!type.IsGenericType)
				return null;

			var genType = type.GetGenericTypeDefinition();

			if(genType == typeof(List<>))
			{
				var itemType = type.GetGenericArguments()[0];
				return Delegate.CreateDelegate(funcType, ForwardListCombineMI.MakeGenericMethod(itemType));
			}

			if(genType == typeof(IList<>))
			{
				var itemType = type.GetGenericArguments()[0];
				return Delegate.CreateDelegate(funcType, ForwardIListCombineMI.MakeGenericMethod(itemType));
			}

			if(genType == typeof(ICollection<>))
			{
				var itemType = type.GetGenericArguments()[0];
				return Delegate.CreateDelegate(funcType, ForwardICollectionCombineMI.MakeGenericMethod(itemType));
			}

			if(genType == typeof(IEnumerable<>))
			{
				var itemType = type.GetGenericArguments()[0];
				return Delegate.CreateDelegate(funcType, ForwardIEnumerableCombineMI.MakeGenericMethod(itemType));
			}
			
			return null;
		}


		internal static readonly MethodInfo ForwardArrayCombineMI = typeof(BuildToolkit).GetMethod("ForwardArrayCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T[] ForwardArrayCombine<T>(T[] x, T[] y)
		{
			if (y == null)
				return x;

			if (x == null)
				return y;

			if (y.Length == 0)
				return x;

			if (x.Length == 0)
				return y;

			return y;
		}

		internal static readonly MethodInfo ForwardListCombineMI = typeof(BuildToolkit).GetMethod("ForwardListCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static List<T> ForwardListCombine<T>(List<T> x, List<T> y)
		{
			if (y == null)
				return x;

			if (x == null)
				return y;

			if(y.Count == 0)
				return x;

			if(x.Count == 0)
				return y;

			return y;
		}

		internal static readonly MethodInfo ForwardIListCombineMI = typeof(BuildToolkit).GetMethod("ForwardIListCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static IList<T> ForwardIListCombine<T>(IList<T> x, IList<T> y)
		{
			if (y == null)
				return x;

			if (x == null)
				return y;

			if (y.Count == 0)
				return x;

			if (x.Count == 0)
				return y;

			return y;
		}

		internal static readonly MethodInfo ForwardICollectionCombineMI = typeof(BuildToolkit).GetMethod("ForwardICollectionCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static ICollection<T> ForwardICollectionCombine<T>(ICollection<T> x, ICollection<T> y)
		{
			if (y == null)
				return x;

			if (x == null)
				return y;

			if (y.Count == 0)
				return x;

			if (x.Count == 0)
				return y;

			return y;
		}

		internal static readonly MethodInfo ForwardIEnumerableCombineMI = typeof(BuildToolkit).GetMethod("ForwardIEnumerableCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static IEnumerable<T> ForwardIEnumerableCombine<T>(IEnumerable<T> x, IEnumerable<T> y)
		{
			if (y == null)
				return x;

			if (x == null)
				return y;

			if (!y.Any())
				return x;

			if (!x.Any())
				return y;

			return y;
		}
		
		internal static readonly MethodInfo FieldCombineMI = typeof(BuildToolkit).GetMethod("FieldCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T FieldCombine<T>(IGenericCombiner combiner, T prev, T next)
		{
			return combiner.Combine<T>(prev, next);
		}
	}
}
