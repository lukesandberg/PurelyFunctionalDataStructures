using System;
using System.Linq;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PurelyFunctional;
namespace PurelyFunctional.Test
{
	[TestClass, PexClass]
	public partial class StackTest
	{
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void StackOrdering<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 5);
			PexAssume.AreDistinct(array, new EqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			IStack<T> s = Stack.New<T>();
			foreach(var t in array)
				s = s.Push(t);
			PexAssert.AreElementsEqual(s, array.Reverse(), new PexEqualityComparison<T>((e1, e2) => e1.Equals(e2)));
			PexAssert.AreElementsEqual(s.Reverse(), array, new PexEqualityComparison<T>((e1, e2) => e1.Equals(e2)));
		}
	}
}
