using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IQueue<T> : IEnumerable<T>
	{
		bool IsEmpty { get; }
		T Peek();
		IQueue<T> Enqueue(T value);
		IQueue<T> Dequeue();
	}

	public sealed class Queue<T> : IQueue<T>
	{
		private sealed class EmptyQueue : IQueue<T>
		{
			public bool IsEmpty	{get { return true; }}
			
			public T Peek() { throw new Exception("Can't peek empty queue");}

			public IQueue<T> Enqueue(T value) 
			{
				return new Queue<T>(Stack<T>.Empty.Push(value), Stack<T>.Empty);
			}

			public IQueue<T> Dequeue() { throw new Exception("Can't dequeue empty queue");}

			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		private static readonly EmptyQueue empty = new EmptyQueue();
		public static IQueue<T> Empty { get { return empty; } }

		private readonly IStack<T> backwards;
		private readonly IStack<T> forwards;

		private Queue(IStack<T> forwards, IStack<T> backwards)
		{
			this.backwards = backwards;
			this.forwards = forwards;
		}

		public bool IsEmpty
		{
			get { return false; }
		}

		public T Peek()
		{
			return forwards.Peek();
		}

		public IQueue<T> Enqueue(T value)
		{
			return new Queue<T>(forwards, backwards.Push(value));
		}

		public IQueue<T> Dequeue()
		{
			IStack<T> f = forwards.Pop();
			if(!f.IsEmpty)
				return new Queue<T>(f, backwards);
			else if(backwards.IsEmpty)
				return Queue<T>.Empty;
			else
				return new Queue<T>(backwards.Reverse(), Stack<T>.Empty);
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach(var t in forwards) yield return t;
			foreach(var t in backwards.Reverse())
				yield return t;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}
}
