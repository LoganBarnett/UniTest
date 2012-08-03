using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GoodStuff.NaturalLanguage;
using NUnit.Framework;

[TestFixture]
public class UniTestEvaluation {
	[Test]
	public void TestSuccess() {
		Assert.That(true);
	}
	
	[Test]
	public void TestFailure() {
		Assert.Fail();
	}
	
	[Test]
	public void TestError() {
		throw new System.Exception("Test exception");
	}
	
	[Test]
	public void TestPending() {
		Assert.Ignore();
	}
}