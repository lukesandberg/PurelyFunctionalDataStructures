using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

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
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void HMQueueOrdering<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			var q = Queue.NewHM<T>().EnqueueAll(array);
			PexAssert.AreElementsEqual(q, array, new PexEqualityComparison<T>((e1, e2) => e1.Equals(e2)));
		}
		[TestMethod]
		public void HMQueueTest()
		{
			var query = Enumerable.Range(1, 100).Select(l => {
				var q = Queue.NewHM<int>().EnqueueAll(Enumerable.Range(0, l));
				int c = 0;
				Exception e = null;
				try
				{
					c = q.Count();
				}
				catch(Exception ex)
				{
					e = ex;
				}
				return new { l, b = c == l && e == null, e};
			});
			foreach(var r in query.Where(r => !r.b).ToList())
			{
				Debug.WriteLine(r.l + (r.e != null ? "\t" + r.e.Message :""));
			}

		}
	}
}
