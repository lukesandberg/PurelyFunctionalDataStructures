using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PurelyFunctional.Test
{
	[TestClass, PexClass]
	public partial class ListTest
	{
		[PexMethod]
		[PexGenericArguments(typeof(int))]
		public void NSListSize<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 100);
			var l = NSList.New<T>();
			foreach(var t in array)
			{
				l = l.Add(t);
			}
			PexAssert.AreEqual(array.Length, l.Size);
		}
		[PexMethod(MaxBranches = 20000)]
		[PexGenericArguments(typeof(int))]
		public void NSListTail<T>([PexAssumeNotNull] T[] array)
		{
			PexAssume.IsTrue(array.Length > 2);
			PexAssume.AreDistinct(array, (t1, t2) => t1.Equals(t2));
			var l = NSList.New<T>();
			foreach(var t in array)
			{
				l = l.Add(t);
			}
			foreach(var t in array.Reverse())
			{
				PexAssert.AreEqual(t, l.Head);
				l = l.Tail;
			}
		}
		
	}
}
