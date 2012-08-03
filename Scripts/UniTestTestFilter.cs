using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GoodStuff.NaturalLanguage;
using NUnit.Core;

public class UniTestTestFilter : ITestFilter {
	UniTestTestNode suite;
	
	public UniTestTestFilter(UniTestTestNode suite) {
		this.suite = suite;
	}
	
	public bool Pass(ITest test) {
		var node = suite.FindNodeByOriginalName(test.TestName.FullName);
		return node != null && node.IsScheduledToRun;
	}

	public bool Match(ITest test) {
		Debug.Log("Match: " + test.TestName.FullName);
		var node = suite.FindNodeByOriginalName(test.TestName.FullName);
		return node != null && node.IsScheduledToRun;
	}

	public bool IsEmpty {
		get {
			return !suite.IsScheduledToRun;
		}
	}
}