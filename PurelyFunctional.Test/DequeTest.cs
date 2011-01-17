using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PurelyFunctional.Test
{
	[TestClass, PexClass]
	public partial class DequeTest
	{
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void DequeOrdering<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			var d = Deque.New<T>();
			foreach(var i in array)
				d = d.EnqueueRight(i);
			foreach(var i in array)
			{
				PexAssert.IsTrue(d.Left.Equals(i));
				d = d.DequeueLeft();
			}
		}
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void DequeReverseOrdering<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			var d = Deque.New<T>();
			foreach(var i in array)
				d = d.EnqueueLeft(i);
			foreach(var i in array)
			{
				PexAssert.IsTrue(d.Right.Equals(i));
				d = d.DequeueRight();
			}
		}

		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void DequeEnumeration<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			var d = Deque.New<T>();
			foreach(var i in array)
				d = d.EnqueueRight(i);
			PexAssert.AreElementsEqual<T>(array, d, (e1, e2) => e1.Equals(e2));
		}
	}
}
