using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GoodStuff.NaturalLanguage;

public class UniTestTestNode {
	bool isScheduledToRun;
	UniTestRunState runState;
	
	public string OriginalName { get; set; }
	public string FriendlyName { get; set; }
	
	public List<UniTestTestNode> Children { get; set;}
	public bool Expanded { get; set; }
	
	public UniTestTestNode() {
		Children = new List<UniTestTestNode>();
		Expanded = true;
	}
	
	public UniTestRunState RunState {
		get {
			if(Children.IsEmpty()) return runState;
			else {
				// probably not very efficient
				if(Children.Any(c => c.RunState == UniTestRunState.Failed)) return UniTestRunState.Failed;
				else if (Children.Any(c => c.RunState == UniTestRunState.Pending)) return UniTestRunState.Pending;
				else if (Children.Any(c => c.RunState == UniTestRunState.Passed)) return UniTestRunState.Passed;
				else return UniTestRunState.NotRun;
			}
		}
		set {
			runState = value;
		}
	}
	
	public bool IsScheduledToRun {
		get {
			if(Children.IsEmpty()) {
				return isScheduledToRun;
			}
			else return Children.All(c => c.IsScheduledToRun);
		}
		
		set {
			isScheduledToRun = value;
			Children.Each(c => c.IsScheduledToRun = value);
		}
	}
	
	public void SetUnscheduledToNotRun() {
		if(!IsScheduledToRun) RunState = UniTestRunState.NotRun;
		Children.Each(c => c.SetUnscheduledToNotRun());
	}
	
	public UniTestTestNode FindNodeByOriginalName(string originalName) {
		if(OriginalName == originalName) return this;
		foreach(var child in Children) {
			var node = child.FindNodeByOriginalName(originalName);
			if(node != null) return node;
		}
		return null;
	}
}