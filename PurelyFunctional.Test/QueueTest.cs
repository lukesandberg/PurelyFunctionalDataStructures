using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PurelyFunctional.Test
{
	[TestClass, PexClass]
	public partial class QueueTest
	{
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void AmortizedQueueOrdering<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			var q = Queue.New<T>().EnqueueAll(array);
			PexAssert.AreElementsEqual(q, array, new PexEqualityComparison<T>((e1, e2) => e1.Equals(e2)));
		}
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void RTQueueOrdering<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			var q = Queue.NewRT<T>().EnqueueAll(array);
			PexAssert.AreElementsEqual(q, array, new PexEqualityComparison<T>((e1, e2) => e1.Equals(e2)));
		}
	}
}
