using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public static class BinaryTreeExtensions
	{
		public static IEnumerable<T> InOrder<T>(this IBinaryTree<T> tree)
		{
			IStack<IBinaryTree<T>> stack = Stack.New<IBinaryTree<T>>();
			for(IBinaryTree<T> current = tree; !current.IsEmpty || !stack.IsEmpty; current = current.Right)
			{
				while(!current.IsEmpty)
				{
					stack = stack.Push(current);
					current = current.Left;
				}
				current = stack.Peek();
				stack = stack.Pop();
				yield return current.Value;
			}
		}
	}
	public interface IBinaryTree<T>
	{
		bool IsEmpty { get; }
		T Value { get; }
		IBinaryTree<T> Left { get; }
		IBinaryTree<T> Right { get; }
	}

	public sealed class BinaryTree<T> : IBinaryTree<T>
	{
		private sealed class EmptyBinaryTree : IBinaryTree<T>
		{
			public bool IsEmpty { get { return true; } }
			public IBinaryTree<T> Left { get { throw new Exception("empty tree"); } }
			public IBinaryTree<T> Right { get { throw new Exception("empty tree"); } }
			public T Value { get { throw new Exception("empty tree"); } }
		}
		private static readonly EmptyBinaryTree empty = new EmptyBinaryTree();
		public static IBinaryTree<T> Empty { get { return empty; } }

		private readonly T value;
		private readonly IBinaryTree<T> left;
		private readonly IBinaryTree<T> right;

		public BinaryTree(T value, IBinaryTree<T> left, IBinaryTree<T> right)
		{
			this.value = value;
			this.left = left ?? Empty;
			this.right = right ?? Empty;
		}

		#region IBinaryTree<T> Members

		public bool IsEmpty { get { return false; } }

		public T Value { get { return value; } }
		public IBinaryTree<T> Left { get { return left; } }
		public IBinaryTree<T> Right { get { return right; } }

		#endregion
	}
}
