// <copyright file="QueueTest.RTQueueOrdering.g.cs" company="Authorized User">Copyright � Authorized User 2010</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Pex.Framework.Exceptions;

namespace PurelyFunctional.Test
{
	public partial class QueueTest
	{
[TestMethod]
[PexGeneratedBy(typeof(QueueTest))]
[PexRaisedException(typeof(PexAssertFailedException))]
public void RTQueueOrderingThrowsPexAssertFailedException14()
{
    int[] ints = new int[6];
    ints[0] = 1;
    ints[1] = 256;
    ints[2] = 8;
    ints[3] = 16;
    ints[5] = 4;
    this.RTQueueOrdering<int>(ints);
}
	}
}
