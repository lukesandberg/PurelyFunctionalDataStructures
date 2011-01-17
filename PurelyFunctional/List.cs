using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IList<T> : IEnumerable<T>
	{
		int Size { get; }
		bool IsEmpty { get; }
		IList<T> Add(T val);
		IList<T> Tail { get; }
		T Head { get; }
	}
	
	public static class NSList
	{
		public static IList<T> New<T>()
		{
			return Empty<T>.EmptyList;
		}
		private interface Tree<T>
		{
			int Size { get; }
		}
		private class Node<T>
		{
			private readonly T first;
			private readonly T second;

			public Node(T f, T s)
			{
				this.first = f;
				this.second = s;
			}
			public T First { get { return first; } }
			public T Second { get { return second; } }
		}
		private abstract class ANSList<T> : IList<T>
		{
			public abstract ANSList<T> Add(T val);
			public abstract ANSList<T> Tail { get; }
			public abstract ANSList<Node<T>> InnerTail { get; }
			public abstract T Head { get; }
			public abstract IEnumerator<T> GetEnumerator();

			#region IList<T> Members

			public abstract int Size { get; }
			public abstract bool IsEmpty { get; }
			IList<T> IList<T>.Add(T val) { return Add(val); }
			IList<T> IList<T>.Tail { get { return Tail; } }
			T IList<T>.Head { get { return Head; } }
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
			#endregion
		}
		private class Empty<T> : ANSList<T>
		{
			public readonly static Empty<T> EmptyList = new Empty<T>();
			private Empty() {/*private constructor*/}

			#region IList<T> Members

			public override int Size { get { return 0; } }
			public override bool IsEmpty { get { return false; } }
			public override ANSList<T> Add(T val) { return new NSListOne<T>(val, Empty<Node<T>>.EmptyList); }
			public override ANSList<T> Tail { get { throw new Exception("cant tail empty"); } }
			public override ANSList<Node<T>> InnerTail { get { throw new Exception("cant tail empty"); } }
			public override T Head { get { throw new Exception("empty list"); } }

			public override IEnumerator<T> GetEnumerator() { yield break; }

			#endregion
		}
		private class NSListZero<T> : ANSList<T>
		{
			private readonly ANSList<Node<T>> tail;

			public NSListZero(ANSList<Node<T>> tail)
			{
				this.tail = tail;
			}

			#region IList<T> Members
			public override int Size { get { return 2 * tail.Size; } }
			public override bool IsEmpty { get { return true; } }
			public override ANSList<T> Add(T val) { return new NSListOne<T>(val, tail); }
			public override ANSList<Node<T>> InnerTail { get { return tail; } }
			public override ANSList<T> Tail { get { return new NSListOne<T>(tail.Head.Second, new NSListZero<Node<T>>(tail.InnerTail)); } }

			public override T Head { get { return tail.Head.First; } }

			public override IEnumerator<T> GetEnumerator()
			{
				foreach(var n in tail)
				{
					yield return n.First;
					yield return n.Second;
				}
			}

			#endregion

		}
		private class NSListOne<T> : ANSList<T>
		{
			private readonly T head;
			private readonly ANSList<Node<T>> tail;

			public NSListOne(T head, ANSList<Node<T>> tail)
			{
				this.head = head;
				this.tail = tail;
			}

			#region IList<T> Members
			public override int Size { get { return 1 + 2 * tail.Size; } }
			public override bool IsEmpty { get { return true; } }
			public override ANSList<T> Add(T val) { return new NSListZero<T>(tail.Add(new Node<T>(val, head))); }
			public override ANSList<T> Tail { get { return new NSListZero<T>(tail); } }
			public override ANSList<Node<T>> InnerTail { get { return tail; } }
			public override T Head { get { return head; } }

			public override IEnumerator<T> GetEnumerator()
			{
				yield return head;
				foreach(var n in tail)
				{
					yield return n.First;
					yield return n.Second;
				}
			}

			#endregion

		}
	}
	public static class SList
	{
		public static IList<T> New<T>()
		{
			return Empty<T>.EmptyList;
		}

		private class Empty<T> : IList<T>
		{
			public readonly static Empty<T> EmptyList = new Empty<T>();
			private Empty()
			{
				//private constructor
			}
			#region IList<T> Members
			public int Size { get { return 0; } }
			public bool IsEmpty { get { return true; } }
			public IList<T> Add(T val) { return new SList<T>(val, this); }
			public IList<T> Tail { get { throw new Exception("cant tail empty"); } }
			public T Head { get { throw new Exception("empty list"); } }
			public IEnumerator<T> GetEnumerator() { yield break; }
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

			#endregion
		}
		private class SList<T> : IList<T>
		{
			private readonly T head;
			private readonly IList<T> tail;

			public SList(T head, IList<T> tail)
			{
				this.head = head;
				this.tail = tail;
			}

			#region IList<T> Members
			public int Size { get { return 1 + tail.Size; } }
			public bool IsEmpty { get { return false; } }
			public IList<T> Add(T val) { return new SList<T>(val, this); }
			public IList<T> Tail { get { return tail; } }
			public T Head { get { return head; } }

			public IEnumerator<T> GetEnumerator()
			{
				yield return head;
				foreach(var i in tail)
					yield return i;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

			#endregion
		}
	}
}
