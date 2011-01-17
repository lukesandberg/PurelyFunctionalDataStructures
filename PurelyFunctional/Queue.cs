using System;
using System.Collections.Generic;
using System.Linq;

namespace PurelyFunctional
{
	public interface IQueue<T> : IEnumerable<T>
	{
		bool IsEmpty { get; }
		T Peek();
		IQueue<T> Enqueue(T value);
		IQueue<T> Dequeue();
	}

	public static class Queue
	{
		public static IQueue<T> New<T>()
		{
			return AmortizedQueue<T>.Empty;
		}
		public static IQueue<T> NewRT<T>()
		{
			return new RealTimeQueue<T>(Stream.New<T>(), Stack.New<T>(), Stream.New<T>());
		}
		public static IQueue<T> EnqueueAll<T>(this IQueue<T> q, IEnumerable<T> collection)
		{
			foreach(var t in collection)
				q = q.Enqueue(t);
			return q;
		}
		private sealed class AmortizedQueue<T> : IQueue<T>
		{
			private sealed class EmptyAmortizedQueue : IQueue<T>
			{
				public bool IsEmpty { get { return true; } }

				public T Peek() { throw new Exception("Can't peek empty queue"); }

				public IQueue<T> Enqueue(T value)
				{
					return new AmortizedQueue<T>(Stack.New<T>().Push(value), Stack.New<T>());
				}

				public IQueue<T> Dequeue() { throw new Exception("Can't dequeue empty queue"); }

				public IEnumerator<T> GetEnumerator()
				{
					yield break;
				}

				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}
			}

			private static readonly EmptyAmortizedQueue empty = new EmptyAmortizedQueue();
			public static IQueue<T> Empty { get { return empty; } }

			private readonly IStack<T> backwards;
			private readonly IStack<T> forwards;

			private AmortizedQueue(IStack<T> forwards, IStack<T> backwards)
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
				return new AmortizedQueue<T>(forwards, backwards.Push(value));
			}

			public IQueue<T> Dequeue()
			{
				IStack<T> f = forwards.Pop();
				if(!f.IsEmpty)
					return new AmortizedQueue<T>(f, backwards);
				else if(backwards.IsEmpty)
					return AmortizedQueue<T>.Empty;
				else
					return new AmortizedQueue<T>(backwards.Reverse(), Stack.New<T>());
			}

			public IEnumerator<T> GetEnumerator()
			{
				foreach(var t in forwards)
					yield return t;
				foreach(var t in backwards.Reverse())
					yield return t;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

		}

		private sealed class RealTimeQueue<T> : IQueue<T>
		{
			private readonly IStream<T> front;
			private readonly IStream<T> schedule;
			private readonly IStack<T> back;

			public RealTimeQueue(IStream<T> f, IStack<T> b, IStream<T> s)
			{
				front = f;
				back = b;
				schedule = s;
			}
			#region IQueue<T> Members

			public bool IsEmpty
			{
				get { return front.IsEmpty; }
			}

			public T Peek()
			{
				return front.First;
			}

			public IQueue<T> Enqueue(T value)
			{
				return Exec(front, back.Push(value), schedule);
			}

			public IQueue<T> Dequeue()
			{
				return Exec(front.Rest, back, schedule);
			}

			#endregion

			#region IEnumerable<T> Members
			public IEnumerator<T> GetEnumerator()
			{
				IQueue<T> q = this;
				while(!q.IsEmpty)
				{
					yield return q.Peek();
					q = q.Dequeue();
				}

			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region Helpers
			private static IQueue<T> Exec(IStream<T> f, IStack<T> r, IStream<T> s)
			{
				if(s.IsEmpty)
				{
					var nf = Rotate(f, r, s);
					return new RealTimeQueue<T>(nf, r, nf);
				}
				else
				{
					return new RealTimeQueue<T>(f, r, s.Rest);
				}
			}
			private static IStream<T> Rotate(IStream<T> f, IStack<T> r, IStream<T> s)
			{
				if(f.IsEmpty)
				{
					return Stream.New<T>(r.Peek(), () => s);
				}
				else
				{
					return Stream.New<T>(f.First, () => Rotate(f.Rest, r.Pop(), Stream.New<T>(r.Peek(), () => s)));
				}
			}
			#endregion
		}
	}
	
}
