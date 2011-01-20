using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace PurelyFunctional.Test
{
	[TestClass]
	public class StreamTest
	{
		public static IStream<V> Zip<T, U, V>(Func<T, U, V> f, IStream<T> list1, IStream<U> list2)
		{
			if(list1.IsEmpty || list2.IsEmpty)
				return Stream.New<V>();
			return Stream.New(f(list1.First, list2.First), () => Zip(f, list1.Rest, list2.Rest));
		}
		public static IStream<T> Map<V, T>(Func<V, T> f, IStream<V> l)
		{
			if(l.IsEmpty)
				return Stream.New<T>();
			return Stream.New(f(l.First), () => Map(f, l.Rest));
		}
		public static IStream<T> Merge<T>(IStream<T> s1, IStream<T> s2) where T : IComparable<T>
		{
			if(s1.IsEmpty)
				return s2;
			else if(s2.IsEmpty)
				return s1;
			int c = s1.First.CompareTo(s2.First);
			if(c < 0)
				return Stream.New(s1.First, () => Merge(s1.Rest, s2));
			else if(c > 0)
				return Stream.New(s2.First, () => Merge(s2.Rest, s1));
			else
				return Stream.New(s1.First, () => Merge(s1.Rest, s2.Rest));
		}

		[TestMethod]
		public void HammingNumberTest()
		{
			IStream<int> ones = null;
			ones = Stream.New(1, () => ones);
			
			IStream<int> nats = null;
			nats = Stream.New(0, () => Zip((x, y) => x+y, ones, nats));

			IStream<int> fibs = null;
			fibs = Stream.New<int>(0, () => Stream.New<int>(1, () => Zip((x,y)=>x+y, fibs, fibs.Rest)));

			IStream<int> hamming = null;
			hamming = Stream.New(1, () => Merge(Merge(Map(x => x * 3, hamming), Map(x => x * 2, hamming)), Map(x => x*5, hamming)));

			
		}
	}
}
