using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IHeap<T>
		where T : IComparable<T>
	{
		bool IsEmpty { get; }
		IHeap<T> Insert(T val);

		T Min { get; }
		IHeap<T> DeleteMin();
	}

	public static class Heap
	{
		public static IHeap<T> Merge<T>(this IHeap<T> h1, IHeap<T> h2)
			where T : IComparable<T>
		{
			return LeftistHeap<T>.Merge(h1, h2);
		}

		public static IHeap<T> ToHeap<T>(this IEnumerable<T> list) 
			where T : IComparable<T>
		{

			var l = list.Select(i => LeftistHeap<T>.Empty.Insert(i)).ToList();
			return l.PairwiseMerge();
		}

		private static IHeap<T> PairwiseMerge<T>(this IEnumerable<IHeap<T>> heaps)
			where T : IComparable<T>
		{
			if(heaps.Count() == 1)
				return heaps.First();
			var evens = heaps.Where((h, i) => i % 2 == 0);
			var odds = heaps.Where((h, i) => i % 2 == 1);
			return evens.PairwiseMerge().Merge(odds.PairwiseMerge());
		}
		public static IHeap<T> New<T>() where T : IComparable<T>
		{
			return LeftistHeap<T>.Empty;
		}

		private sealed class LeftistHeap<T> : IHeap<T> where T : IComparable<T>
		{
			internal static IHeap<T> Merge(IHeap<T> h1, IHeap<T> h2)
			{
				if(h1.IsEmpty)
					return h2;
				if(h2.IsEmpty)
					return h1;
				var lh1 = (LeftistHeap<T>) h1;
				var lh2 = (LeftistHeap<T>) h2;
				if(h1.Min.CompareTo(h2.Min) <= 0)
					return MakeHeap(h1.Min, lh1.left, Merge(h2, lh1.right));
				else
					return MakeHeap(h2.Min, lh2.left, Merge(h1, lh2.right));
			}
			private static uint Rank(IHeap<T> h)
			{
				if(h.IsEmpty)
					return 0;
				return ((LeftistHeap<T>) h).rank;
			}
			private static IHeap<T> MakeHeap(T value, IHeap<T> a, IHeap<T> b)
			{
				var rankA = Rank(a);
				var rankB = Rank(b);
				if(rankA >= rankB)
					return new LeftistHeap<T>(rankB + 1, value, a, b);
				return new LeftistHeap<T>(rankA + 1, value, b, a);
			}
			private sealed class EmptyHeap : IHeap<T>
			{
				#region IHeap<T> Members

				public bool IsEmpty { get { return true; } }
				public IHeap<T> Insert(T val) { return new LeftistHeap<T>(1, val, this, this); }
				public T Min { get { throw new Exception("empty heap"); } }
				public IHeap<T> DeleteMin() { throw new Exception("empty heap"); }

				#endregion
			}

			private readonly static EmptyHeap empty = new EmptyHeap();
			public static IHeap<T> Empty { get { return empty; } }

			private readonly uint rank;
			private readonly T value;
			private readonly IHeap<T> left;
			private readonly IHeap<T> right;

			private LeftistHeap(uint rank, T value, IHeap<T> left, IHeap<T> right)
			{
				this.rank = rank;
				this.value = value;
				this.left = left;
				this.right = right;
			}

			#region IHeap<T> Members

			public bool IsEmpty { get { return false; } }

			public IHeap<T> Insert(T val)
			{
				if(val.CompareTo(value) <= 0)
					return new LeftistHeap<T>(rank + 1, val, this, empty);
				return MakeHeap(value, left, right.Insert(val));
			}

			public T Min { get { return value; } }

			public IHeap<T> DeleteMin() { return Merge(left, right); }

			#endregion
		}

	}
}
