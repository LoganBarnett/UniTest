using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GoodStuff.NaturalLanguage;
using NUnit.Core;
using NUnit.Util;
using NUnit.UiKit;

public class UniTestEditorRunner : EditorWindow {
	NUnit.Util.DefaultTestRunnerFactory testFactory;
	NUnit.Util.TestLoader loader;
	TestRunner testRunner;
	UniTestTestNode suite;
	
	Texture passedTexture;
	Texture pendingTexture;
	Texture failedTexture;
	Texture notRunTexture;
	
	Vector2 scrollPosition;
	
	[MenuItem("Window/UniTest Runner %#u")]
	public static void CreateWindow() {
		EditorWindow.GetWindow(typeof(UniTestEditorRunner));
	}
	
	public void OnEnable() {
		passedTexture  = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/UniTest/Textures/RunState-Passed.psd", typeof(Texture));
		pendingTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/UniTest/Textures/RunState-Pending.psd", typeof(Texture));
		failedTexture  = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/UniTest/Textures/RunState-Failed.psd", typeof(Texture));
		notRunTexture  = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/UniTest/Textures/RunState-NotRun.psd", typeof(Texture));
		
		ServiceManager.Services.ClearServices();
		ServiceManager.Services.AddService(new DomainManager());
        ServiceManager.Services.AddService(new RecentFilesService());
        ServiceManager.Services.AddService(new ProjectService());
        ServiceManager.Services.AddService(new TestLoader(new GuiTestEventDispatcher()));
        ServiceManager.Services.AddService(new AddinRegistry());
        ServiceManager.Services.AddService(new AddinManager());
        ServiceManager.Services.AddService(new TestAgency());
		
		ServiceManager.Services.InitializeServices();
		
		Services.UserSettings.SaveSetting("Options.TestLoader.ReloadOnChange", true);
		
		loader = Services.TestLoader;
		loader.Events.ProjectLoadFailed += (path, e) => {
			Debug.Log(e);
			Debug.Log(e.Exception.Message);
			Debug.Log(e.Exception.StackTrace);
		};
		var assembly = System.Reflection.Assembly.GetAssembly(typeof(UniTestEditorRunner));
		
		loader.Events.TestLoadFailed += (file, exception) => {
			Debug.Log(file);
			Debug.Log(exception);
			Debug.Log(exception.Exception.Message);
			Debug.Log(exception.Exception.StackTrace);
		};
			
		
		loader.LoadProject(assembly.Location);
		var factory = new InProcessTestRunnerFactory();
		var package = new TestPackage(assembly.Location);
		package.Settings["DomainUsage"] = DomainUsage.None;
		testRunner = factory.MakeTestRunner(package);
		testRunner.Load(package);

		
		suite = BuildTestSuiteHierarchy(testRunner.Test);
	}
	
	public void OnGUI() {
		var dimensions = new Rect(0f, 0f, position.width, position.height);
		GUILayout.BeginArea(dimensions); {
			if(suite == null) {
				GUILayout.Label("Error loading test assembly. See error log for details");
				return;
			}
			GUILayout.BeginHorizontal(); {
				if(GUILayout.Button("Run Selected Tests")) {
					suite.SetUnscheduledToNotRun();
					testRunner.Run(new UniTestEventListener(suite), new UniTestTestFilter(suite), true, LoggingThreshold.Error);
					
				}
			} GUILayout.EndHorizontal();
		
			scrollPosition = GUILayout.BeginScrollView(scrollPosition); {
				DrawNode(suite);
			} GUILayout.EndScrollView();
		} GUILayout.EndArea();
		
	}
	
	void DrawNode(UniTestTestNode node) {
		if(!node.Children.IsEmpty() && EditorGUILayout.Foldout(node.Expanded, "")) {
			node.Expanded = true;
			GUILayout.BeginHorizontal(); {

				DrawRunState(node.RunState);
				var newScheduledToRun = GUILayout.Toggle(node.IsScheduledToRun, node.FriendlyName);
				if(newScheduledToRun != node.IsScheduledToRun) node.IsScheduledToRun = newScheduledToRun;
				GUILayout.FlexibleSpace();
			} GUILayout.EndHorizontal(); 
				GUILayout.BeginVertical(); {
					GUILayout.BeginHorizontal(); {
					
					GUILayout.Space(30);
						GUILayout.BeginVertical(); {
							foreach(var childNode in node.Children) {
								DrawNode(childNode);
							}
						} GUILayout.EndVertical();
					} GUILayout.EndHorizontal();
				} GUILayout.EndVertical();
			
		}
		else {
			GUILayout.BeginHorizontal(); {
				node.Expanded = false;
				DrawRunState(node.RunState);
				var newScheduledToRun = GUILayout.Toggle(node.IsScheduledToRun, node.FriendlyName);
				if(newScheduledToRun != node.IsScheduledToRun) node.IsScheduledToRun = newScheduledToRun;
				GUILayout.FlexibleSpace();
			} GUILayout.EndHorizontal();
		}
	}
	
	void DrawRunState(UniTestRunState state) {
		Texture texture = null;
		switch(state) {
		case UniTestRunState.NotRun:
			texture = notRunTexture;
			break;
		case UniTestRunState.Failed:
			texture = failedTexture;
			break;
		case UniTestRunState.Pending:
			texture = pendingTexture;
			break;
		case UniTestRunState.Passed:
			texture = passedTexture;
			break;
		default:
			throw new System.Exception(string.Format("RunState '{0}' not supported!", state));
		}
		GUILayout.Label(texture);
	}
	
	UniTestTestNode BuildTestSuiteHierarchy(ITest test) {
		var node = new UniTestTestNode();
		node.FriendlyName = test.TestName.Name;
		node.OriginalName = test.TestName.FullName;
		
		if(test.Tests == null) return node;
		foreach(ITest childTest in test.Tests) {
			var childNode = BuildTestSuiteHierarchy(childTest);
			node.Children.Add(childNode);
		}
		return node;
	}
}