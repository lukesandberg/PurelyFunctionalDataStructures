using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IStack<T> : IEnumerable<T>
	{
		bool IsEmpty { get; }
		IStack<T> Push(T value);
		IStack<T> Pop();
		T Peek();
	}

	public static class Stack
	{
		public static IStack<T> Reverse<T>(this IStack<T> stack)
		{
			if(stack == null)
				throw new ArgumentNullException("stack");
			IStack<T> r = FullStack<T>.Empty;
			for(IStack<T> f = stack; !f.IsEmpty; f = f.Pop())
				r = r.Push(f.Peek());
			return r;
		}
		
		public static IStack<T> New<T>()
		{
			return FullStack<T>.Empty;
		}

		private sealed class FullStack<T> : IStack<T>
		{
			private sealed class EmptyStack : IStack<T>
			{
				public bool IsEmpty { get { return true; } }

				public IStack<T> Push(T value) { return new FullStack<T>(value, this); }

				public IStack<T> Pop() { throw new Exception("Can't pop an empty stack"); }
				public T Peek() { throw new Exception("Can't peek an empty stack"); }

				public IEnumerator<T> GetEnumerator() { yield break; }

				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
			}

			private static readonly EmptyStack empty = new EmptyStack();
			public static IStack<T> Empty { get { return empty; } }

			private readonly T head;
			private readonly IStack<T> tail;
			private FullStack(T head, IStack<T> tail)
			{
				this.head = head;
				this.tail = tail;
			}

			public bool IsEmpty
			{
				get { return false; }
			}

			public IStack<T> Push(T value)
			{
				return new FullStack<T>(value, this);
			}

			public IStack<T> Pop()
			{
				return tail;
			}

			public T Peek()
			{
				return head;
			}

			public IEnumerator<T> GetEnumerator()
			{
				for(IStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
					yield return stack.Peek();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
	

	
}
