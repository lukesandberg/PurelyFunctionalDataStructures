// <copyright file="HeapTest.MinTest.g.cs" company="Authorized User">Copyright � Authorized User 2010</copyright>
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
	public partial class HeapTest
	{
[TestMethod]
[PexGeneratedBy(typeof(HeapTest))]
public void MinTest598()
{
    int[] ints = new int[0];
    this.MinTest<int>(ints);
}
[TestMethod]
[PexGeneratedBy(typeof(HeapTest))]
public void MinTest470()
{
    int[] ints = new int[1];
    this.MinTest<int>(ints);
}
[TestMethod]
[PexGeneratedBy(typeof(HeapTest))]
public void MinTest924()
{
    int[] ints = new int[2];
    ints[0] = 2;
    this.MinTest<int>(ints);
}
	}
}
