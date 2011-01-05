using System;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PurelyFunctional;

namespace PurelyFunctional.Test
{
	[TestClass, PexClass]
	public partial class HeapTest
	{
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void MinTest<T>([PexAssumeNotNull] T[] arr)
			where T : IComparable<T>
		{
			IHeap<T> h = arr.ToHeap();
			PexAssert.Case(arr.Length == 0)
				.Implies(() => h.IsEmpty)
			.Case(arr.Length > 0)
				.Implies(() => h.Min.Equals(arr.Min()));
		}
	}
}
