// <copyright file="QueueTest.HMQueueOrdering.g.cs" company="Authorized User">Copyright � Authorized User 2010</copyright>
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
public void HMQueueOrderingThrowsPexAssertFailedException670()
{
    int[] ints = new int[6];
    ints[1] = 264;
    ints[2] = 272;
    ints[3] = 320;
    ints[4] = 256;
    ints[5] = 257;
    this.HMQueueOrdering<int>(ints);
}
[TestMethod]
[PexGeneratedBy(typeof(QueueTest))]
public void HMQueueOrdering15()
{
    int[] ints = new int[9];
    ints[1] = 1;
    ints[2] = 2;
    ints[3] = 512;
    ints[4] = 3;
    ints[5] = 513;
    ints[6] = 272;
    ints[7] = 256;
    ints[8] = 257;
    this.HMQueueOrdering<int>(ints);
}
[TestMethod]
[PexGeneratedBy(typeof(QueueTest))]
public void HMQueueOrdering557()
{
    int[] ints = new int[8];
    ints[0] = 512;
    ints[1] = 8;
    ints[2] = 128;
    ints[3] = 1;
    ints[5] = 529;
    ints[6] = 16;
    ints[7] = 257;
    this.HMQueueOrdering<int>(ints);
}
[TestMethod]
[PexGeneratedBy(typeof(QueueTest))]
public void HMQueueOrdering103()
{
    int[] ints = new int[24];
    ints[0] = 64;
    ints[1] = 16;
    ints[2] = 9;
    ints[3] = 100;
    ints[4] = 1;
    ints[5] = 67;
    ints[6] = 129;
    ints[7] = 66;
    ints[8] = 193;
    ints[9] = 68;
    ints[10] = 65;
    ints[11] = 102;
    ints[12] = 585;
    ints[13] = 320;
    ints[14] = 73;
    ints[15] = 225;
    ints[16] = 69;
    ints[17] = 33;
    ints[18] = 132;
    ints[20] = 12;
    ints[21] = 4;
    ints[22] = 140;
    ints[23] = 5;
    this.HMQueueOrdering<int>(ints);
}
	}
}
