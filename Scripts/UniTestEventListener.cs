using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GoodStuff.NaturalLanguage;
using NUnit.Core;

public class UniTestEventListener : EventListener {
	UniTestTestNode suite;
	
	public UniTestEventListener(UniTestTestNode suite) {
		this.suite = suite;
	}
	
	void EventListener.RunStarted(string name, int testCount) {
//		throw new System.NotImplementedException();
	}

	void EventListener.RunFinished(TestResult result) {
//		Debug.Log("Run finished " + result.FullName);
	}

	public void RunFinished(System.Exception exception) {
//		Debug.Log("Run finished " + exception.Message);
	}

	void EventListener.TestStarted(TestName testName) {
//		Debug.Log("Test started " + testName.FullName);
	}

	void EventListener.TestFinished(TestResult result) {
		try {
			var node = suite.FindNodeByOriginalName(result.FullName);
			
			switch(result.ResultState) {
			case ResultState.Success:
				node.RunState = UniTestRunState.Passed;
				break;
			case ResultState.Skipped:
			case ResultState.Ignored:
			case ResultState.Inconclusive:
				node.RunState = UniTestRunState.Pending;
				break;
			case ResultState.Failure:
			case ResultState.Error:
				node.RunState = UniTestRunState.Failed;
				break;
			}
		}
		catch(System.Exception e) {
			Debug.LogError("Error with " + result.FullName);
			Debug.LogError(e.Message);
			Debug.LogError(e.StackTrace);
		}
//		Debug.Log("Test finished " + result.FullName);
//		Debug.Log("Result of finished test: " + result.ResultState);
	}

	void EventListener.SuiteStarted(TestName testName) {
//		Debug.Log("Suite started " + testName.FullName);
	}

	void EventListener.SuiteFinished(TestResult result) {
//		Debug.Log("Suite Finished " + result.FullName);
	}

	void EventListener.UnhandledException(System.Exception exception) {
//		Debug.Log("Unhandled exception " + exception.Message);
	}

	void EventListener.TestOutput(TestOutput testOutput) {
//		Debug.Log("Test Output " + testOutput.Text);
	}
}