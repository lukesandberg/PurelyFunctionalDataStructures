using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IBinarySearchTree<K, V> : IBinaryTree<V>, IMap<K, V> where K : IComparable<K>
	{
		K Key { get; }
		new IBinarySearchTree<K, V> Left { get; }
		new IBinarySearchTree<K, V> Right { get; }
		new IBinarySearchTree<K, V> Add(K key, V value);
		new IBinarySearchTree<K, V> Remove(K key);
		IBinarySearchTree<K, V> Search(K key);
	}
	internal enum NodeColor
	{
		RED,
		BLACK
	}
	public class RedBlackTree<K, V> : IBinarySearchTree<K, V> where K : IComparable<K>
	{
		private sealed class EmptyRedBlackTree : IBinarySearchTree<K, V>
		{
			#region IBinarySearchTree Members
			public K Key { get { throw new Exception("empty tree"); } }

			public IBinarySearchTree<K, V> Left { get { throw new Exception("empty tree"); } }

			public IBinarySearchTree<K, V> Right { get { throw new Exception("empty tree"); } }

			public IBinarySearchTree<K, V> Add(K key, V value)
			{
				return new RedBlackTree<K, V>(key, value, NodeColor.RED, this, this);
			}

			public IBinarySearchTree<K, V> Remove(K key) { throw new Exception("empty tree"); }

			public IBinarySearchTree<K, V> Search(K key)
			{
				return this;
			}

			#endregion

			#region IBinaryTree<V> Members

			public bool IsEmpty
			{
				get { return true; }
			}

			public V Value { get { throw new Exception("empty tree"); } }

			IBinaryTree<V> IBinaryTree<V>.Left { get { throw new Exception("empty tree"); } }
			IBinaryTree<V> IBinaryTree<V>.Right { get { throw new Exception("empty tree"); } }

			#endregion

			#region IMap<K,V> Members

			public bool Contains(K key)
			{
				return false;
			}

			public V Lookup(K key) { throw new Exception("empty tree"); }
			IMap<K, V> IMap<K, V>.Add(K key, V value) { return Add(key, value); }

			IMap<K, V> IMap<K, V>.Remove(K key) { return Remove(key); }

			public IEnumerable<K> Keys
			{
				get { yield break; }
			}

			public IEnumerable<V> Values
			{
				get { yield break; }
			}

			public IEnumerable<KeyValuePair<K, V>> Pairs
			{
				get { yield break; }
			}

			#endregion

		}
		

		private static readonly EmptyRedBlackTree empty = new EmptyRedBlackTree();
		public static IBinarySearchTree<K, V> Empty { get { return empty; } }

		private readonly NodeColor color;
		private readonly IBinarySearchTree<K, V> left;
		private readonly IBinarySearchTree<K, V> right;
		private readonly K key;
		private readonly V value;

		private RedBlackTree(K key, V value, NodeColor c, IBinarySearchTree<K, V> left, IBinarySearchTree<K, V> right)
		{
			this.key = key;
			this.value = value;
			this.color = c;
			this.left = left;
			this.right = right;
		}

		#region IBinarySearchTree<K,V> Members

		public K Key { get { return key; } }

		public IBinarySearchTree<K, V> Left { get { return left; } }

		public IBinarySearchTree<K, V> Right { get { return right; } }

		public IBinarySearchTree<K, V> Add(K key, V value)
		{
			Func<IBinarySearchTree<K, V>, RedBlackTree<K, V>> ins = null;
			ins = t =>
				{
					if(t.IsEmpty)
					{
						return new RedBlackTree<K, V>(key, value, NodeColor.RED, t, t);
					}
					else if(t is RedBlackTree<K, V>)
					{
						var rbt = (RedBlackTree<K, V>) t;
						int comp = key.CompareTo(rbt.Key);
						if(comp == 0)
							return rbt;
						else if(comp < 0)
							return Balance(rbt.color, ins(rbt.Left), t.Key, t.Value, t.Right);
						else
							return Balance(rbt.color, t.Left, t.Key, t.Value, ins(t.Right));
					}
					else
					{
						//can't balance it if its not a RBTree
						t = t.Add(key, value);
						return new RedBlackTree<K, V>(t.Key, t.Value, NodeColor.RED, t.Left, t.Right);
					}
				};
			var tree = ins(this);
			return new RedBlackTree<K, V>(tree.Key, tree.Value, NodeColor.BLACK, tree.Left, tree.Right);
		}

		public IBinarySearchTree<K, V> Remove(K key)
		{
			throw new NotImplementedException();
		}
		public IBinarySearchTree<K, V> Search(K key)
		{
			int comp = key.CompareTo(this.key);
			if(comp == 0)
				return this;
			else if(comp < 0)
				return left.Search(key);
			else
				return right.Search(key);
		}

		#endregion

		#region IBinaryTree<V> Members

		public bool IsEmpty
		{
			get { return false; }
		}

		public V Value
		{
			get { return value; }
		}

		IBinaryTree<V> IBinaryTree<V>.Left
		{
			get { return left; }
		}

		IBinaryTree<V> IBinaryTree<V>.Right
		{
			get { return right; }
		}

		#endregion

		#region IMap<K,V> Members

		public bool Contains(K key)
		{
			IBinarySearchTree<K, V> tree = Search(key);
			if(tree.IsEmpty)
				return false;
			return true;
		}

		public V Lookup(K key)
		{
			IBinarySearchTree<K, V> tree = Search(key);
			if(tree.IsEmpty)
				throw new Exception("key not found");
			return tree.Value;
		}

		IMap<K, V> IMap<K, V>.Add(K key, V value)
		{
			return Add(key, value);
		}

		IMap<K, V> IMap<K, V>.Remove(K key)
		{
			return Remove(key);
		}

		public IEnumerable<K> Keys
		{
			get { return Enumerate().Select(t => t.Key); }
		}

		public IEnumerable<V> Values
		{
			get { return Enumerate().Select(t => t.Value); }
		}

		public IEnumerable<KeyValuePair<K, V>> Pairs
		{
			get { return Enumerate().Select(t => new KeyValuePair<K, V>(t.Key, t.Value)); }
		}

		#endregion

		#region Helpers
		private IEnumerable<IBinarySearchTree<K, V>> Enumerate()
		{
			var stack = Stack<IBinarySearchTree<K, V>>.Empty;
			for(IBinarySearchTree<K, V> current = this; !current.IsEmpty || !stack.IsEmpty; current = current.Right)
			{
				while(!current.IsEmpty)
				{
					stack = stack.Push(current);
					current = current.Left;
				}
				current = stack.Peek();
				stack = stack.Pop();
				yield return current;
			}
		}

		private static bool IsLLDoubleRed<KEY, VAL>(NodeColor color, IBinarySearchTree<KEY, VAL> left, KEY key, VAL value, IBinarySearchTree<KEY, VAL> right,
			ref IBinarySearchTree<KEY, VAL> a, ref IBinarySearchTree<KEY, VAL> b, ref IBinarySearchTree<KEY, VAL> c, ref IBinarySearchTree<KEY, VAL> d,
			ref KEY kx, ref VAL vx, ref KEY ky, ref VAL vy, ref KEY kz, ref VAL vz)
				where KEY : IComparable<KEY>
		{
			var rb_left = left as RedBlackTree<KEY, VAL>;
			if(rb_left != null && rb_left.color == NodeColor.RED)
			{
				var rb_left_left = rb_left.Left as RedBlackTree<KEY, VAL>;
				if(rb_left_left != null && rb_left_left.color == NodeColor.RED)
				{
					a = rb_left_left.Left;
					b = rb_left_left.Right;
					c = rb_left.Right;
					d = right;
					kx = rb_left_left.Key;
					vx = rb_left_left.Value;
					ky = rb_left.Key;
					vy = rb_left.Value;
					kz = key;
					vz = value;
					return true;
				}
			}
			return false;
		}
		private static bool IsLRDoubleRed<KEY, VAL>(NodeColor color, IBinarySearchTree<KEY, VAL> left, KEY key, VAL value, IBinarySearchTree<KEY, VAL> right,
			ref IBinarySearchTree<KEY, VAL> a, ref IBinarySearchTree<KEY, VAL> b, ref IBinarySearchTree<KEY, VAL> c, ref IBinarySearchTree<KEY, VAL> d,
			ref KEY kx, ref VAL vx, ref KEY ky, ref VAL vy, ref KEY kz, ref VAL vz)
				where KEY : IComparable<KEY>
		{
			var rb_left = left as RedBlackTree<KEY, VAL>;
			if(rb_left != null && rb_left.color == NodeColor.RED)
			{
				var rb_left_right = rb_left.Right as RedBlackTree<KEY, VAL>;
				if(rb_left_right != null && rb_left_right.color == NodeColor.RED)
				{
					a = rb_left.Left;
					b = rb_left_right.Left;
					c = rb_left_right.Right;
					d = right;
					kx = rb_left.Key;
					vx = rb_left.Value;
					ky = rb_left_right.Key;
					vy = rb_left_right.Value;
					kz = key;
					vz = value;
					return true;
				}
			}
			return false;
		}

		private static bool IsRLDoubleRed<KEY, VAL>(NodeColor color, IBinarySearchTree<KEY, VAL> left, KEY key, VAL value, IBinarySearchTree<KEY, VAL> right,
	ref IBinarySearchTree<KEY, VAL> a, ref IBinarySearchTree<KEY, VAL> b, ref IBinarySearchTree<KEY, VAL> c, ref IBinarySearchTree<KEY, VAL> d,
	ref KEY kx, ref VAL vx, ref KEY ky, ref VAL vy, ref KEY kz, ref VAL vz)
		where KEY : IComparable<KEY>
		{
			var rb_right = right as RedBlackTree<KEY, VAL>;
			if(rb_right != null && rb_right.color == NodeColor.RED)
			{
				var rb_right_left = rb_right.Left as RedBlackTree<KEY, VAL>;
				if(rb_right_left != null && rb_right_left.color == NodeColor.RED)
				{
					a = left;
					b = rb_right_left.Left;
					c = rb_right_left.Right;
					d = right;
					kx = key;
					vx = value;
					ky = rb_right_left.Key;
					vy = rb_right_left.Value;
					kz = rb_right.Key;
					vz = rb_right.Value;
					return true;
				}
			}
			return false;
		}
		private static bool IsRRDoubleRed<KEY, VAL>(NodeColor color, IBinarySearchTree<KEY, VAL> left, KEY key, VAL value, IBinarySearchTree<KEY, VAL> right,
	ref IBinarySearchTree<KEY, VAL> a, ref IBinarySearchTree<KEY, VAL> b, ref IBinarySearchTree<KEY, VAL> c, ref IBinarySearchTree<KEY, VAL> d,
	ref KEY kx, ref VAL vx, ref KEY ky, ref VAL vy, ref KEY kz, ref VAL vz)
		where KEY : IComparable<KEY>
		{
			var rb_right = right as RedBlackTree<KEY, VAL>;
			if(rb_right != null && rb_right.color == NodeColor.RED)
			{
				var rb_right_right = rb_right.Right as RedBlackTree<KEY, VAL>;
				if(rb_right_right != null && rb_right_right.color == NodeColor.RED)
				{
					a = left;
					b = rb_right_right.Left;
					c = rb_right_right.Right;
					d = right;
					kx = key;
					vx = value;
					ky = rb_right.Key;
					vy = rb_right.Value;
					kz = rb_right_right.Key;
					vz = rb_right_right.Value;
					return true;
				}
			}
			return false;
		}

		private static RedBlackTree<KEY, VAL> Balance<KEY, VAL>(NodeColor color, IBinarySearchTree<KEY, VAL> left, KEY key, VAL value, IBinarySearchTree<KEY, VAL> right) where KEY : IComparable<KEY>
		{
			IBinarySearchTree<KEY, VAL> a = null, b = null, c = null, d = null;
			KEY kx = default(KEY), ky = default(KEY), kz = default(KEY);
			VAL vx = default(VAL), vy = default(VAL), vz = default(VAL);
			if(color == NodeColor.BLACK)
			{
				if(IsLLDoubleRed<KEY, VAL>(color, left, key, value, right, ref a, ref b, ref c, ref d, ref kx, ref vx, ref ky, ref vy, ref kz, ref vz)
					|| IsLRDoubleRed<KEY, VAL>(color, left, key, value, right, ref a, ref b, ref c, ref d, ref kx, ref vx, ref ky, ref vy, ref kz, ref vz)
					|| IsRLDoubleRed<KEY, VAL>(color, left, key, value, right, ref a, ref b, ref c, ref d, ref kx, ref vx, ref ky, ref vy, ref kz, ref vz)
					|| IsRRDoubleRed<KEY, VAL>(color, left, key, value, right, ref a, ref b, ref c, ref d, ref kx, ref vx, ref ky, ref vy, ref kz, ref vz))
				{
					return new RedBlackTree<KEY, VAL>(ky, vy, NodeColor.RED, new RedBlackTree<KEY, VAL>(kx, vx, NodeColor.BLACK, a, b), new RedBlackTree<KEY, VAL>(kz, vz, NodeColor.BLACK, c, d));
				}
			}
			return new RedBlackTree<KEY, VAL>(key, value, color, left, right);
		}
		#endregion
	}
}
