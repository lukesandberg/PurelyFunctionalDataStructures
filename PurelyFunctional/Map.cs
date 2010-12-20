using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IMap<K, V> where K : IComparable<K>
	{
		bool Contains(K key);
		V Lookup(K key);
		IMap<K, V> Add(K key, V value);
		IMap<K, V> Remove(K key);
		IEnumerable<K> Keys { get; }
		IEnumerable<V> Values { get; }
		IEnumerable<KeyValuePair<K, V>> Pairs { get; }
	}
	    
}
