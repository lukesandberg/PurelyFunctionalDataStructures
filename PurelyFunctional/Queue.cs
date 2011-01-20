using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PurelyFunctional
{
	public interface IQueue<T> : IEnumerable<T>
	{
		bool IsEmpty { get; }
		
		T Head { get; }
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
		public static IQueue<T> NewHM<T>()
		{
			return HoodMelvilleQueue<T>.Empty;
		}
		public static IQueue<T> EnqueueAll<T>(this IQueue<T> q, IEnumerable<T> collection)
		{
			foreach(var t in collection)
				q = q.Enqueue(t);
			return q;
		}
		#region Amortized
		
		private sealed class AmortizedQueue<T> : IQueue<T>
		{
			private sealed class EmptyAmortizedQueue : IQueue<T>
			{
				public bool IsEmpty { get { return true; } }

				public T Head { get { throw new Exception("Can't peek empty queue"); } }

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

			public T Head
			{
				get
				{
					return forwards.Peek();
				}
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
		#endregion
		#region Hood Melville Real Time Queue
		#region Rotation State
		private interface IRotationState<T>
		{
			bool IsDone { get; }
			IRotationState<T> Exec();
			IStack<T> Result { get; }
			IRotationState<T> Invalidate();
		}
		private sealed class ReversingState<T> : IRotationState<T>
		{
			private readonly uint valid;
			private readonly IStack<T> f_orig;
			private readonly IStack<T> f_rev;
			private readonly IStack<T> b_orig;
			private readonly IStack<T> b_rev;

			private ReversingState(uint valid, IStack<T> f_orig, IStack<T> f_rev, IStack<T> b_orig, IStack<T> b_rev)
			{
				this.valid = valid;
				this.f_orig = f_orig;
				this.f_rev = f_rev;
				this.b_orig = b_orig;
				this.b_rev = b_rev;
			}
			public ReversingState(IStack<T> f_orig, IStack<T> b_orig)
				: this(0, f_orig, Stack.New<T>(), b_orig, Stack.New<T>())
			{
			}
			#region IRotationState<T> Members

			public bool IsDone { get { return false; } }
			public IRotationState<T> Exec()
			{
				if(f_orig.IsEmpty)
					return new AppendingState<T>(valid, f_rev, b_rev.Push(b_orig.Peek()));
				return new ReversingState<T>(valid + 1, f_orig.Pop(), f_rev.Push(f_orig.Peek()), b_orig.Pop(), b_rev.Push(b_orig.Peek()));
			}

			public IStack<T> Result { get { throw new Exception("Not Done!"); } }

			public IRotationState<T> Invalidate()
			{
				return new ReversingState<T>(valid - 1, f_orig, f_rev, b_orig, b_rev);
			}
			#endregion
		}
		private sealed class AppendingState<T> : IRotationState<T>
		{
			private readonly uint valid;
			private readonly IStack<T> from;
			private readonly IStack<T> to;

			public AppendingState(uint valid, IStack<T> from, IStack<T> to)
			{
				this.valid = valid;
				this.to = to;
				this.from = from;
			}

			#region IRotationState<T> Members

			public bool IsDone { get { return false; } }
			public IRotationState<T> Exec()
			{
				if(valid == 0)
					return new DoneState<T>(to);
				return new AppendingState<T>(valid - 1, from.Pop(), to.Push(from.Peek()));
			}

			public IStack<T> Result { get { throw new Exception("Not Done!"); } }

			public IRotationState<T> Invalidate()
			{
				if(valid == 0)
					return new DoneState<T>(to.Pop());
				return new AppendingState<T>(valid - 1, from, to);
			}

			#endregion
		}
		private sealed class DoneState<T> : IRotationState<T>
		{
			private readonly IStack<T> result;
			public DoneState(IStack<T> result)
			{
				this.result = result;
			}

			#region IRotationState<T> Members
			public bool IsDone { get { return true; } }
			public IRotationState<T> Exec() { return this; }
			public IStack<T> Result { get { return result; } }
			public IRotationState<T> Invalidate() { return this; }

			#endregion
		}
		private sealed class IdleState<T> : IRotationState<T>
		{
			public static readonly IdleState<T> Idle = new IdleState<T>();
			private IdleState() { }

			#region IRotationState<T> Members
			public bool IsDone { get { return false; } }
			public IRotationState<T> Exec() { return this; }
			public IStack<T> Result { get { throw new Exception("Idle state"); } }
			public IRotationState<T> Invalidate() { return this; }

			#endregion
		}
		#endregion
		private sealed class HoodMelvilleQueue<T> : IQueue<T>
		{
			public static readonly HoodMelvilleQueue<T> Empty = new HoodMelvilleQueue<T>(0, Stack.New<T>(), IdleState<T>.Idle, 0, Stack.New<T>());

			private readonly uint length_front;
			private readonly IStack<T> front;
			private readonly IRotationState<T> state;
			private readonly IStack<T> back;
			private readonly uint length_back;

			public HoodMelvilleQueue(uint length_front, IStack<T> front, IRotationState<T> state, uint length_back, IStack<T> back)
			{
				this.length_front = length_front;
				this.front = front;
				this.state = state;
				this.length_back = length_back;
				this.back = back;
			}

			private static HoodMelvilleQueue<T> Exec2(uint length_front, IStack<T> front, IRotationState<T> state, uint length_back, IStack<T> back)
			{
				var newstate = state.Exec().Exec();
				if(newstate.IsDone)
					return new HoodMelvilleQueue<T>(length_front, newstate.Result, IdleState<T>.Idle, length_back, back);
				return new HoodMelvilleQueue<T>(length_front, front, newstate, length_back, back);
			}
			private static HoodMelvilleQueue<T> Check(uint length_front, IStack<T> front, IRotationState<T> state, uint length_back, IStack<T> back)
			{
				if(length_back <= length_front)
					return Exec2(length_front, front, state, length_back, back);
				var newstate = new ReversingState<T>(front, back);
				return Exec2(length_front + length_back, front, newstate, 0, Stack.New<T>());
			}

			#region IQueue<T> Members
			public bool IsEmpty { get { return length_front == 0; } }

			public T Head
			{
				get
				{
					if(front.IsEmpty)
						throw new Exception("Empty queue");
					return front.Peek();
				}
			}

			public IQueue<T> Enqueue(T value)
			{
				return Check(length_front, front, state, length_back + 1, back.Push(value));
			}

			public IQueue<T> Dequeue()
			{
				if(front.IsEmpty)
					throw new Exception("Empty queue");
				return Check(length_front - 1, front.Pop(), state.Invalidate(), length_back, back);
			}

			#endregion

			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				IQueue<T> q = this;
				while(!q.IsEmpty)
				{
					yield return q.Head;
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
		}
		  
		#endregion

		#region Real Time
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

			public T Head
			{
				get
				{
					return front.First;
				}
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
					yield return q.Head;
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
					return Stream.New(r.Peek(), s.Memoize());
				}
				else
				{
					return Stream.New(f.First, () => Rotate(f.Rest, r.Pop(), Stream.New(r.Peek(), s.Memoize())));
				}
			}
			#endregion
		}
		#endregion
	}

}
