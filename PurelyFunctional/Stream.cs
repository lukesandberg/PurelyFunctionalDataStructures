using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IStream<T> : IEnumerable<T>
	{
		bool IsEmpty { get; }
		T First { get; }
		IStream<T> Rest { get; }
		IStream<T> Add(T val);
	}


	public static class Stream
	{
		public static IStream<T> ToStream<T>(this IEnumerable<T> l)
		{
			return l.GetEnumerator().ToStream();
		}
		public static IStream<T> ToStream<T>(this IEnumerator<T> e)
		{
			if(e.MoveNext())
				return New(e.Current, () => e.ToStream());
			return New<T>();
		}
		public static IStream<T> New<T>()
		{
			return EmptyStream<T>.Empty;
		}
		public static IStream<T> New<T>(T first)
		{
			return new FullStream<T>(first, ()=>EmptyStream<T>.Empty);
		}
		
		public static IStream<T> New<T>(T val, IMemoized<IStream<T>> rest)
		{
			return new FullStream<T>(val, rest);
		}
		public static IStream<T> New<T>(T val, Func<IStream<T>> rest)
		{
			return new FullStream<T>(val, rest);
		}
		public static IStream<T> Append<T>(this IStream<T> str1, IStream<T> str2)
		{
			if(str1.IsEmpty)
				return str2;
			else
				return New<T>(str1.First, () => str1.Rest.Append(str2));
		}
		private sealed class EmptyStream<T> : IStream<T>
		{
			private static readonly EmptyStream<T> empty = new EmptyStream<T>();
			public static IStream<T> Empty { get { return empty; } }

			#region IStream<T> Members
			public bool IsEmpty { get { return true; } }

			public T First { get { throw new Exception("empty stream"); } }

			public IStream<T> Rest { get { throw new Exception("empty stream"); } }

			public IStream<T> Add(T val)
			{
				return new FullStream<T>(val, () => Empty);
			}

			#endregion

			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}
		private class FullStream<T> : IStream<T>
		{
			private readonly T first;
			private readonly IMemoized<IStream<T>> rest;
			public FullStream(T first, IMemoized<IStream<T>> rest)
			{
				this.first = first;
				this.rest = rest;
			}
			public FullStream(T first, Func<IStream<T>> rest) : this(first, rest.Memoize())
			{
			}
			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				for(IStream<T> r = this; !r.IsEmpty; r = r.Rest)
				{
					yield return r.First;
				}
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region IStream<T> Members

			public bool IsEmpty { get { return false; } }

			public T First { get { return first; } }

			public IStream<T> Rest { get { return rest.Value; } }

			public IStream<T> Add(T val) { return new FullStream<T>(val, () => this); }

			#endregion
		}
	}
}
