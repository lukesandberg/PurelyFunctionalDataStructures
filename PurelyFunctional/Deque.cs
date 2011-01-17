using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IDeque<T> : IEnumerable<T>
	{
		T Left { get; }
		T Right { get; }
		bool IsEmpty { get; }
		IDeque<T> EnqueueLeft(T val);
		IDeque<T> EnqueueRight(T val);
		IDeque<T> DequeueLeft();
		IDeque<T> DequeueRight();
		IDeque<T> Append(IDeque<T> other);
	}

	public static class Deque
	{
		public static IDeque<T> New<T>()
		{
			return Empty<T>.EmptyTree;
		}
		#region Nodes
		private static class Node
		{
			public static Node<T> One<T>(T a) { return new One<T>(a); }
			public static Node<T> Two<T>(T a, T b) { return new Two<T>(a, b); }
			public static Node<T> Three<T>(T a, T b, T c) { return new Three<T>(a, b, c); }
			public static Node<T> Four<T>(T a, T b, T c, T d) { return new Four<T>(a, b, c, d); }
		}

		private abstract class Node<T> : IEnumerable<T>
		{
			public virtual bool IsFull { get { return false; } }
			public abstract int Size { get; }
			public abstract T Left { get; }
			public abstract T Right { get; }
			public abstract Node<T> EnqueueLeft(T val);
			public abstract Node<T> EnqueueRight(T val);
			public abstract Node<T> DequeueLeft();
			public abstract Node<T> DequeueRight();

			#region IEnumerable<T> Members

			public abstract IEnumerator<T> GetEnumerator();
			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}

		private class One<T> : Node<T>
		{
			private readonly T value;
			public One(T value)
			{
				this.value = value;
			}

			public override int Size { get { return 1; } }
			public override T Left { get { return value; } }
			public override T Right { get { return value; } }
			public override Node<T> EnqueueLeft(T left) { return new Two<T>(left, value); }
			public override Node<T> EnqueueRight(T right) { return new Two<T>(value, right); }
			public override Node<T> DequeueLeft() { throw new Exception("can't dequeue single node"); }
			public override Node<T> DequeueRight() { throw new Exception("can't dequeue single node"); }
			public override IEnumerator<T> GetEnumerator()
			{
				yield return value;
			}
		}
		private class Two<T> : Node<T>
		{
			private readonly T left;
			private readonly T right;
			public Two(T left, T right)
			{
				this.right = right;
				this.left = left;
			}

			public override int Size { get { return 2; } }
			public override T Left { get { return left; } }
			public override T Right { get { return right; } }
			public override Node<T> EnqueueLeft(T new_left) { return new Three<T>(new_left, left, right); }
			public override Node<T> EnqueueRight(T new_right) { return new Three<T>(left, right, new_right); }
			public override Node<T> DequeueLeft() { return new One<T>(right); }
			public override Node<T> DequeueRight() { return new One<T>(left); }
			public override IEnumerator<T> GetEnumerator()
			{
				yield return left;
				yield return right;
			}
		}
		private class Three<T> : Node<T>
		{
			private readonly T left;
			private readonly T middle;
			private readonly T right;
			public Three(T left, T middle, T right)
			{
				this.left = left;
				this.middle = middle;
				this.right = right;
			}

			public override int Size { get { return 3; } }
			public override T Left { get { return left; } }
			public override T Right { get { return right; } }
			public override Node<T> EnqueueLeft(T new_left) { return new Four<T>(new_left, left, middle, right); }
			public override Node<T> EnqueueRight(T new_right) { return new Four<T>(left, middle, right, new_right); }
			public override Node<T> DequeueLeft() { return new Two<T>(middle, right); }
			public override Node<T> DequeueRight() { return new Two<T>(left, middle); }
			public override IEnumerator<T> GetEnumerator()
			{
				yield return left;
				yield return middle;
				yield return right;
			}
		}
		private class Four<T> : Node<T>
		{
			private readonly T left;
			private readonly T left_middle;
			private readonly T right_middle;
			private readonly T right;

			public Four(T left, T left_middle, T right_middle, T right)
			{
				this.left = left;
				this.left_middle = left_middle;
				this.right_middle = right_middle;
				this.right = right;
			}

			public override bool IsFull { get { return true; } }
			public override int Size { get { return 4; } }
			public override T Left { get { return left; } }
			public override T Right { get { return right; } }
			public override Node<T> EnqueueLeft(T left) { throw new Exception("can't enqueue a 4 node"); }
			public override Node<T> EnqueueRight(T right) { throw new Exception("can't enqueue a 4 node"); }
			public override Node<T> DequeueLeft() { return new Three<T>(left_middle, right_middle, right); }
			public override Node<T> DequeueRight() { return new Three<T>(left, left_middle, right_middle); }
			public override IEnumerator<T> GetEnumerator()
			{
				yield return left;
				yield return left_middle;
				yield return right_middle;
				yield return right;
			}
		}
		#endregion
		#region Finger Tree
		private abstract class FTree<T> : IDeque<T>
		{
			public abstract FTree<T> EnqueueLeft(T val);
			public abstract FTree<T> EnqueueRight(T val);
			public abstract FTree<T> DequeueLeft();
			public abstract FTree<T> DequeueRight();
			public FTree<T> EnqueueLeft(IEnumerable<T> vals)
			{
				var tree = this;
				foreach(var val in vals.Reverse())
					tree = tree.EnqueueLeft(val);
				return tree;
			}
			public FTree<T> EnqueueRight(IEnumerable<T> vals)
			{
				var tree = this;
				foreach(var val in vals)
					tree = tree.EnqueueRight(val);
				return tree;
			}
			#region concatenation double dispatch
			public abstract FTree<T> AppendRight(IEnumerable<T> others, FTree<T> other);
			public abstract FTree<T> AppendLeft(FTree<T> other, IEnumerable<T> others);
			public abstract FTree<T> AppendRightDeep(IEnumerable<T> others, Deep<T> other);
			public abstract FTree<T> AppendRightSingle(IEnumerable<T> others, Single<T> other);
			public abstract FTree<T> AppendRightEmpty(IEnumerable<T> others, Empty<T> other);
			public abstract FTree<T> AppendLeftDeep(Deep<T> other, IEnumerable<T> others);
			public abstract FTree<T> AppendLeftSingle(Single<T> other, IEnumerable<T> others);
			public abstract FTree<T> AppendLeftEmpty(Empty<T> other, IEnumerable<T> others);
			#endregion
			#region IDeque<T> Members

			IDeque<T> IDeque<T>.EnqueueLeft(T val) { return EnqueueLeft(val); }
			IDeque<T> IDeque<T>.EnqueueRight(T val) { return EnqueueRight(val); }
			IDeque<T> IDeque<T>.DequeueLeft() { return DequeueLeft(); }
			IDeque<T> IDeque<T>.DequeueRight() { return DequeueRight(); }
			IDeque<T> IDeque<T>.Append(IDeque<T> other) { return other is FTree<T> ? AppendRight(Enumerable.Empty<T>(), (FTree<T>) other) : other.Append(this); }
			public abstract T Left { get; }
			public abstract T Right { get; }
			public abstract bool IsEmpty { get; }

			#endregion

			#region IEnumerable<T> Members

			public abstract IEnumerator<T> GetEnumerator();

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}
		private sealed class Empty<T> : FTree<T>
		{
			private static readonly Empty<T> empty = new Empty<T>();
			public static Empty<T> EmptyTree { get { return empty; } }
			private Empty() {/*private constructor*/ }
			public override bool IsEmpty { get { return true; } }
			public override T Left { get { throw new Exception("Empty Dequeue"); } }
			public override T Right { get { throw new Exception("Empty Dequeue"); } }
			public override FTree<T> EnqueueLeft(T val) { return new Single<T>(val); }
			public override FTree<T> EnqueueRight(T val) { return new Single<T>(val); }
			public override FTree<T> DequeueLeft() { throw new Exception("Empty Dequeue"); }
			public override FTree<T> DequeueRight() { throw new Exception("Empty Dequeue"); }
			public override IEnumerator<T> GetEnumerator() { yield break; }

			#region concatenation double dispatch
			public override FTree<T> AppendRight(IEnumerable<T> others, FTree<T> other) { return other.EnqueueLeft(others); }
			public override FTree<T> AppendLeft(FTree<T> other, IEnumerable<T> others) { return other.EnqueueRight(others); }

			public override FTree<T> AppendRightDeep(IEnumerable<T> others, Deep<T> other) { return other.EnqueueLeft(others); }
			public override FTree<T> AppendLeftDeep(Deep<T> other, IEnumerable<T> others) { return other.EnqueueRight(others); }

			public override FTree<T> AppendRightSingle(IEnumerable<T> others, Single<T> other) { return other.EnqueueLeft(others); }
			public override FTree<T> AppendLeftSingle(Single<T> other, IEnumerable<T> others) { return other.EnqueueRight(others); }

			public override FTree<T> AppendRightEmpty(IEnumerable<T> others, Empty<T> other) { return other.EnqueueLeft(others); }
			public override FTree<T> AppendLeftEmpty(Empty<T> other, IEnumerable<T> others) { return other.EnqueueRight(others); }
			#endregion
		}

		private sealed class Single<T> : FTree<T>
		{
			private readonly T value;
			public Single(T value)
			{
				this.value = value;
			}
			public override bool IsEmpty { get { return false; } }
			public override T Left { get { return value; } }
			public override T Right { get { return value; } }
			public override FTree<T> EnqueueLeft(T nl) { return new Deep<T>(Node.One(nl), Empty<Node<T>>.EmptyTree, Node.One(value)); }
			public override FTree<T> EnqueueRight(T nr) { return new Deep<T>(Node.One(value), Empty<Node<T>>.EmptyTree, Node.One(nr)); }
			public override FTree<T> DequeueLeft() { return Empty<T>.EmptyTree; }
			public override FTree<T> DequeueRight() { return Empty<T>.EmptyTree; }
			public override IEnumerator<T> GetEnumerator() { yield return value; }

			#region concatenation double dispatch
			public override FTree<T> AppendRight(IEnumerable<T> others, FTree<T> other) { return other.AppendLeftSingle(this, others); }
			public override FTree<T> AppendLeft(FTree<T> other, IEnumerable<T> others) { return other.AppendRightSingle(others, this); }

			public override FTree<T> AppendRightDeep(IEnumerable<T> others, Deep<T> other) { return other.EnqueueLeft(others).EnqueueLeft(value); }
			public override FTree<T> AppendLeftDeep(Deep<T> other, IEnumerable<T> others) { return other.EnqueueRight(others).EnqueueRight(value); }

			public override FTree<T> AppendRightSingle(IEnumerable<T> others, Single<T> other) { return other.EnqueueLeft(others).EnqueueLeft(value); }
			public override FTree<T> AppendLeftSingle(Single<T> other, IEnumerable<T> others) { return other.EnqueueRight(others).EnqueueRight(value); }

			public override FTree<T> AppendRightEmpty(IEnumerable<T> others, Empty<T> other) { return EnqueueRight(others); }
			public override FTree<T> AppendLeftEmpty(Empty<T> other, IEnumerable<T> others) { return EnqueueLeft(others); }
			#endregion
		}

		private sealed class Deep<T> : FTree<T>
		{
			private readonly Node<T> left;
			private readonly FTree<Node<T>> middle;
			private readonly Node<T> right;

			public Deep(Node<T> left, FTree<Node<T>> middle, Node<T> right)
			{
				this.left = left;
				this.middle = middle;
				this.right = right;
			}

			public override bool IsEmpty { get { return false; } }
			public override T Left { get { return left.Left; } }
			public override T Right { get { return right.Right; } }

			public override FTree<T> EnqueueLeft(T val)
			{
				if(!left.IsFull)
				{
					return new Deep<T>(left.EnqueueLeft(val), middle, right);
				}
				var nl = new Two<T>(val, left.Left);
				var nm = middle.EnqueueLeft(left.DequeueLeft());
				var nr = right;
				return new Deep<T>(nl, nm, nr);
			}

			public override FTree<T> EnqueueRight(T val)
			{
				if(!right.IsFull)
				{
					return new Deep<T>(left, middle, right.EnqueueRight(val));
				}

				var nr = new Two<T>(right.Right, val);
				var nm = middle.EnqueueRight(right.DequeueRight());
				var nl = left;
				return new Deep<T>(nl, nm, nr);
			}

			public override FTree<T> DequeueLeft()
			{
				if(left.Size > 1)
					return new Deep<T>(left.DequeueLeft(), middle, right);
				if(!middle.IsEmpty)
					return new Deep<T>(middle.Left, middle.DequeueLeft(), right);
				if(right.Size > 1)
					return new Deep<T>(new One<T>(right.Left), middle, right.DequeueLeft());
				return new Single<T>(right.Left);
			}

			public override FTree<T> DequeueRight()
			{
				if(right.Size > 1)
					return new Deep<T>(left, middle, right.DequeueRight());
				if(!middle.IsEmpty)
					return new Deep<T>(left, middle.DequeueRight(), middle.Right);
				if(left.Size > 1)
					return new Deep<T>(left.DequeueRight(), middle, new One<T>(left.Right));
				return new Single<T>(left.Right);
			}

			public override IEnumerator<T> GetEnumerator()
			{
				foreach(var i in left)
					yield return i;

				foreach(var ni in middle)
					foreach(var i in ni)
						yield return i;

				foreach(var i in right)
					yield return i;
			}

			#region concatenation double dispatch
			public override FTree<T> AppendRight(IEnumerable<T> others, FTree<T> other) { return other.AppendLeftDeep(this, others); }
			public override FTree<T> AppendLeft(FTree<T> other, IEnumerable<T> others) { return other.AppendRightDeep(others, this); }

			public override FTree<T> AppendRightDeep(IEnumerable<T> others, Deep<T> other) { return new Deep<T>(this.left, this.middle.AppendRight(Nodes(this.right.Concat(others).Concat(other.left).ToStream()), other.middle), other.right); }
			public override FTree<T> AppendLeftDeep(Deep<T> other, IEnumerable<T> others) { return new Deep<T>(other.left, other.middle.AppendRight(Nodes(other.right.Concat(others).Concat(this.left).ToStream()), this.middle), this.right); }

			public override FTree<T> AppendRightSingle(IEnumerable<T> others, Single<T> other) { return this.EnqueueRight(others).EnqueueRight(other.Left); }
			public override FTree<T> AppendLeftSingle(Single<T> other, IEnumerable<T> others) { return this.EnqueueLeft(others).EnqueueLeft(other.Left); }

			public override FTree<T> AppendRightEmpty(IEnumerable<T> others, Empty<T> other) { return EnqueueRight(others); }
			public override FTree<T> AppendLeftEmpty(Empty<T> other, IEnumerable<T> others) { return EnqueueLeft(others); }

			private static IStream<Node<TOther>> Nodes<TOther>(IStream<TOther> els)
			{
				var sa = els.ToStream();
				if(sa.IsEmpty)
					throw new Exception("should be at least two");
				var a = sa.First;

				var sb = sa.Rest;
				if(sb.IsEmpty)
					throw new Exception("should be at least two");
				var b = sb.First;

				var sc = sb.Rest;
				if(sc.IsEmpty)
					return Stream.New(Node.Two(a, b));
				var c = sc.First;

				var sd = sc.Rest;
				if(sd.IsEmpty)
					return Stream.New(Node.Three(a, b, c));
				var d = sd.First;

				var se = sd.Rest;
				if(sd.IsEmpty)
					return Stream.New(Node.Two(a, b), Stream.New(Node.Two(c, d)).Memoize());
				return Stream.New(Node.Three(a, b, c), () => Nodes(sd));
			}
			#endregion
		}
		#endregion
	}
}
