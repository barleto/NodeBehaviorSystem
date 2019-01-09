using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(CutScene))]
public class CutSceneEditor : Editor {

	CutScene cutScene;
	private int index1,index2;
	private bool grouping = false;
	private bool changeOrder = false;
	private CutSceneNodes nodeToChange = null;
	private int directionToChange = 0;
	private List<string> listOfSwitches = new List<string>();
	private string csToLoad;

	enum TypesOfNode{Animation,Dialogue,DialogueNonStop,ExecuteFunctionNode,SetActiveNode,SetSwitchValue,DecisionOnSwitchNode,WaitForSecondsNode,PlayAudioNode,FunctionWithParametersNode, FadeInNode, FadeOutNode};
	private TypesOfNode type = TypesOfNode.Animation;

	public override void OnInspectorGUI(){
		cutScene = (CutScene)target;
		if(cutScene.nodeList == null){
			cutScene.nodeList = new List<CutSceneNodes>();
			cutScene.csnl = ScriptableObject.CreateInstance<CutSceneNodeList>();
			cutScene.csnl.nodeList = cutScene.nodeList;
			createCutSceneAsset();

		}

		//CutSceneSystem
		cutScene.css = GameObject.Find("CutSceneSystem").GetComponent<CutSceneSystem>();
		cutScene.css = (CutSceneSystem)EditorGUILayout.ObjectField ("Cut Scene System",cutScene.css, typeof(CutSceneSystem), true);
		cutScene.csnl = (CutSceneNodeList)EditorGUILayout.ObjectField ("Cut Scene Asset:",cutScene.csnl, typeof(CutSceneNodeList), true);
		if(cutScene.csnl != null){
			cutScene.nodeList = cutScene.csnl.nodeList;
		}


		//switch system here:
		listOfSwitches.Clear ();
		listOfSwitches.Add ("None");
		foreach(GameSwitch gameSwitch in cutScene.css.SwitchVariables){
			listOfSwitches.Add(gameSwitch.name);
		}
		cutScene.indexOfSwitch = EditorGUILayout.Popup ("Switch to check: ",cutScene.indexOfSwitch,listOfSwitches.ToArray());
		if (cutScene.css.SwitchVariables.Count > 0 && (cutScene.indexOfSwitch -1) >= 0) {
			cutScene.gameSwitch = cutScene.css.SwitchVariables [cutScene.indexOfSwitch - 1];
		} else {
			cutScene.gameSwitch = null;
		}

		//draw bool options
		bool pauseGame = GUILayout.Toggle (cutScene.pauseGame,"Pause The Game",GUILayout.Width(300));
		cutScene.pauseGame = pauseGame;

		//show list
		grouping = false;
		foreach(CutSceneNodes nodeS in cutScene.nodeList){
			GUILayout.BeginVertical("box");

			if(!(nodeS is CompositeCutSceneNode)){
				GUILayout.BeginHorizontal ();
				if(GUILayout.Button("- Delete",GUILayout.Width(100))){
					AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(nodeS));
					cutScene.nodeList.Remove(nodeS);
				}
				if(GUILayout.Button("[MOVE UP]",GUILayout.Width(100))){
					changeOrder = true;
					nodeToChange = nodeS;
					directionToChange = -1;
				}
				if(GUILayout.Button("[MOVE DOWN]",GUILayout.Width(100))){
					changeOrder = true;
					nodeToChange = nodeS;
					directionToChange = 1;
				}
				GUILayout.EndHorizontal();
				nodeS.createUIDescription(cutScene,serializedObject);
				if(cutScene.nodeList.IndexOf(nodeS)+1 < cutScene.nodeList.Count){
					if(GUILayout.Button("Group With Next Node")){
						grouping = true;
						index1 = cutScene.nodeList.IndexOf(nodeS);
						index2 = index1+1;
					}
				}
			}else{
				CompositeCutSceneNode nodeC = (CompositeCutSceneNode)nodeS;
				GUILayout.BeginHorizontal ();
				if(GUILayout.Button("[MOVE UP]",GUILayout.Width(100))){
					changeOrder = true;
					nodeToChange = nodeS;
					directionToChange = -1;
				}
				if(GUILayout.Button("[MOVE DOWN]",GUILayout.Width(100))){
					changeOrder = true;
					nodeToChange = nodeS;
					directionToChange = 1;
				}
				GUILayout.EndHorizontal();
				if(nodeC.children.Count>0){
					for(int j=0;j<nodeC.children.Count;j++){
						CutSceneNodes nodeSC = nodeC.children[j];
						if(GUILayout.Button("- Delete",GUILayout.Width(100))){
							AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(nodeSC));
							nodeC.children.Remove(nodeSC);
						}
						nodeSC.createUIDescription(cutScene,serializedObject);
						if(j+1 < nodeC.children.Count){
							if(GUILayout.Button("-- Break Group Here --")){
								CompositeCutSceneNode newComp = new CompositeCutSceneNode();
								newComp.children = nodeC.children.GetRange(j+1,nodeC.children.Count - 1 - j);
								nodeC.children.RemoveRange(j+1,nodeC.children.Count - 1 - j);
								int index = cutScene.nodeList.IndexOf(nodeS);
								cutScene.nodeList.Insert(index+1,newComp);
							}
						}
					}
				}

				if(cutScene.nodeList.IndexOf(nodeS)+1 < cutScene.nodeList.Count){
					if(GUILayout.Button("Group With Next Node")){
						grouping = true;
						index1 = cutScene.nodeList.IndexOf(nodeS);
						index2 = index1+1;
					}
				}

			}
			//nodeS.playWithNext = EditorGUILayout.Toggle("Play With Next: ",nodeS.playWithNext);
			GUILayout.EndVertical();
			EditorUtility.SetDirty(cutScene);
		}

		//draw buttons for adding and subtracting the cutscene sequence:
		GUILayout.BeginHorizontal ();
		type = (TypesOfNode)EditorGUILayout.EnumPopup ("Type of node: ", type);
		if(GUILayout.Button("Add Node",GUILayout.Width(100))){
			switch(type){
			case TypesOfNode.Animation:
				AnimationNode newAnimation = ScriptableObject.CreateInstance<AnimationNode>();
				newAnimation.cutScene = cutScene;
				cutScene.nodeList.Add(newAnimation);
				createNodeAsset(newAnimation);
				break;
			case TypesOfNode.Dialogue:
				Dialogue newDialogue = ScriptableObject.CreateInstance<Dialogue>();
				newDialogue.cutScene = cutScene;
				cutScene.nodeList.Add(newDialogue);
				createNodeAsset(newDialogue);
				break;
			case TypesOfNode.DialogueNonStop:
				DialogueNonStop newDialogueNonStop = ScriptableObject.CreateInstance<DialogueNonStop>();
				newDialogueNonStop.cutScene = cutScene;
				cutScene.nodeList.Add(newDialogueNonStop);
				createNodeAsset(newDialogueNonStop);
				break;
			case TypesOfNode.ExecuteFunctionNode:
				ExecuteFunctionNode newExecuteFunctionNode = ScriptableObject.CreateInstance<ExecuteFunctionNode>();
				newExecuteFunctionNode.cutScene = cutScene;
				cutScene.nodeList.Add(newExecuteFunctionNode);
				createNodeAsset(newExecuteFunctionNode);
				break;
			case TypesOfNode.SetActiveNode:
				SetActiveNode newSetActiveNode = ScriptableObject.CreateInstance<SetActiveNode>();
				newSetActiveNode.cutScene = cutScene;
				cutScene.nodeList.Add(newSetActiveNode);
				createNodeAsset(newSetActiveNode);
				break;
			case TypesOfNode.SetSwitchValue:
				SetSwitchValueNode newSetSwitchValueNode = ScriptableObject.CreateInstance<SetSwitchValueNode>();
				newSetSwitchValueNode.cutScene = cutScene;
				cutScene.nodeList.Add(newSetSwitchValueNode);
				createNodeAsset(newSetSwitchValueNode);
				break;
			case TypesOfNode.DecisionOnSwitchNode:
				DecisionSwitchNode newDecisionSwitchNode = ScriptableObject.CreateInstance<DecisionSwitchNode>();
				newDecisionSwitchNode.cutScene = cutScene;
				cutScene.nodeList.Add(newDecisionSwitchNode);
				createNodeAsset(newDecisionSwitchNode);
				break;
			case TypesOfNode.WaitForSecondsNode:
				WaitForTimeNode newWaitForSeconds = ScriptableObject.CreateInstance<WaitForTimeNode>();
				newWaitForSeconds.cutScene = cutScene;
				cutScene.nodeList.Add(newWaitForSeconds);
				createNodeAsset(newWaitForSeconds);
				break;
			case TypesOfNode.PlayAudioNode:
				PlayAudio newPlayAudio = ScriptableObject.CreateInstance<PlayAudio>();
				newPlayAudio.cutScene = cutScene;
				cutScene.nodeList.Add(newPlayAudio);
				createNodeAsset(newPlayAudio);
				break;
			case TypesOfNode.FunctionWithParametersNode:
				FunctionWithParametersNode newFunctionWithParametersNode = ScriptableObject.CreateInstance<FunctionWithParametersNode>();
				newFunctionWithParametersNode.cutScene = cutScene;
				cutScene.nodeList.Add(newFunctionWithParametersNode);
				createNodeAsset(newFunctionWithParametersNode);
				break;
			case TypesOfNode.FadeInNode:
				FadeInSpriteNode newFadeInSpriteNode = ScriptableObject.CreateInstance<FadeInSpriteNode>();
				newFadeInSpriteNode.cutScene = cutScene;
				cutScene.nodeList.Add(newFadeInSpriteNode);
				createNodeAsset(newFadeInSpriteNode);
				break;
			case TypesOfNode.FadeOutNode:
				FadeOutSpriteNode newFadeOutSpriteNode = ScriptableObject.CreateInstance<FadeOutSpriteNode>();
				newFadeOutSpriteNode.cutScene = cutScene;
				cutScene.nodeList.Add(newFadeOutSpriteNode);
				createNodeAsset(newFadeOutSpriteNode);
				break;
				
			}
		}
		/*if(GUILayout.Button("+Dialogo",GUILayout.Width(100))){
			Dialogue newDialogue = new Dialogue();
			newDialogue.cutScene = cutScene;
			cutScene.nodeList.Add(newDialogue);
			//EditorUtility.SetDirty(newDialogue);
		}
		if(GUILayout.Button("+Animation",GUILayout.Width(100))){
			AnimationNode newAnimation = new AnimationNode();
			newAnimation.cutScene = cutScene;
			cutScene.nodeList.Add(newAnimation);
		}*/
		GUILayout.EndHorizontal ();

		if(grouping){
			CutSceneNodes g1  = cutScene.nodeList[index1];
			CutSceneNodes g2 = cutScene.nodeList[index2];
			if(g1 is CompositeCutSceneNode){
				if(g2 is CompositeCutSceneNode){
					((CompositeCutSceneNode)g1).children.AddRange(((CompositeCutSceneNode)g2).children);
					cutScene.nodeList.Remove(g2);
				}else{
					((CompositeCutSceneNode)g1).children.Add(g2);
					cutScene.nodeList.Remove(g2);
				}
			}else{
				if(g2 is CompositeCutSceneNode){
					((CompositeCutSceneNode)g2).children.Insert(0,g1);
					cutScene.nodeList.Remove(g1);
				}else{
					CompositeCutSceneNode composite = new CompositeCutSceneNode();
					composite.children.Add(g1);
					composite.children.Add(g2);
					cutScene.nodeList.Insert(index2+1,composite);
					cutScene.nodeList.Remove(g1);
					cutScene.nodeList.Remove(g2);
				}
			}
			grouping = false;
		}

		foreach(CutSceneNodes nodeS in cutScene.nodeList){
			if(nodeS is CompositeCutSceneNode){
				if(((CompositeCutSceneNode)nodeS).children.Count < 1){
					cutScene.nodeList.Remove(nodeS);
				}else if(((CompositeCutSceneNode)nodeS).children.Count == 1){
					int index = cutScene.nodeList.IndexOf(nodeS);
					cutScene.nodeList.Insert(index+1,((CompositeCutSceneNode)nodeS).children[0]);
					cutScene.nodeList.Remove(nodeS);
				}
			}
		}
		//changeOrderHere
		if(changeOrder){
			int indexOld = cutScene.nodeList.IndexOf(nodeToChange);
			int newIndex = indexOld + directionToChange;
			if( newIndex >= 0 && newIndex < cutScene.nodeList.Count){
				CutSceneNodes n = cutScene.nodeList[newIndex];
				if(indexOld > newIndex){
					cutScene.nodeList.RemoveAt(indexOld);
					cutScene.nodeList.RemoveAt(newIndex);
					cutScene.nodeList.Insert(newIndex,nodeToChange);
					cutScene.nodeList.Insert(indexOld,n);
				}else{
					cutScene.nodeList.RemoveAt(newIndex);
					cutScene.nodeList.RemoveAt(indexOld);
					cutScene.nodeList.Insert(indexOld,n);
					cutScene.nodeList.Insert(newIndex,nodeToChange);
				}

			}

		}
		//finish changing order
		changeOrder = false;
		nodeToChange = null;
		directionToChange = 0;
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties();
		}

	}

	[MenuItem("Cut Scene System/Add CutScene Component to gameObject")]
	static void addCutsceneComponentToGameObject(){
		GameObject obj = (GameObject)Selection.activeGameObject;
		if(obj!=null){
			Undo.AddComponent(obj,typeof(CutScene));
		}
	}

	// Validate the menu item defined by the function above.
	// The menu item will be disabled if this function returns false.
	[MenuItem ("Cut Scene System/Add CutScene Component to gameObject", true)]
	static bool ValidateaddCutsceneComponentToGameObject () {
		// Return false if no transform is selected.
		return (Selection.activeGameObject != null);
	}

	[MenuItem("Cut Scene System/Add Cut Scene System To Scene")]
	static void CreateCutSceneSytem (MenuCommand command) {
		if(GameObject.Find("CutSceneSystem") == null){
			GameObject sys= Instantiate(Resources.Load("CutSceneSystem", typeof(GameObject))) as GameObject;
			sys.name = "CutSceneSystem";
			Undo.RegisterCreatedObjectUndo(sys, "Create " + sys.name);
		}
	}

	[MenuItem("Cut Scene System/Add Cut Scene System To Scene",true)]
	static bool ValidateCreateCutSceneSytem (MenuCommand command) {
		if(GameObject.Find("CutSceneSystem") == null){
			return true;
		}
		return false;
	}


	void createCutSceneAsset(){
		int fileName = 0;
		while(AssetDatabase.IsValidFolder("Assets/CutSceneSystem/CutScenes/CutScene"+fileName)){
			fileName++;
		}
		AssetDatabase.CreateFolder ("Assets/CutSceneSystem/CutScenes","CutScene"+fileName);
		AssetDatabase.CreateAsset (cutScene.csnl,"Assets/CutSceneSystem/CutScenes/CutScene"+fileName+"/CutScene"+fileName+".asset");
		AssetDatabase.CreateFolder ("Assets/CutSceneSystem/CutScenes/CutScene"+fileName,"nodes");
		/*while(AssetDatabase.LoadAssetAtPath<CutSceneNodeList>("Assets/CutSceneSystem/CutScenes/CutScene"+fileName+".asset") != null){
			fileName++;
		}*/
	}

	void createNodeAsset(CutSceneNodes node){
		string url = AssetDatabase.GetAssetPath (cutScene.csnl);
		int index = AssetDatabase.GetAssetPath (cutScene.csnl).LastIndexOf ("/");
		url = url.Substring (0,index);
		int fileName = 0;
		while(AssetDatabase.LoadAssetAtPath<CutSceneNodes>(url+"/nodes/node"+fileName+".asset") != null){
			fileName++;
		}
		AssetDatabase.CreateAsset (node,url+"/nodes/node"+fileName+".asset");
	}
}



/*
[CustomEditor(typeof(CutScene))]
public class CutSceneEditor : Editor {

	CutScene cutScene;
	private int index1,index2;
	private bool grouping = false;

	public override void OnInspectorGUI(){
		cutScene = (CutScene)target;
		if(cutScene.nodeList == null){
			cutScene.nodeList = new List<CutSceneNodes>();
		}
		//CutSceneSystem
		cutScene.css = GameObject.Find("CutSceneSystem").GetComponent<CutSceneSystem>();
		cutScene.css = (CutSceneSystem)EditorGUILayout.ObjectField ("Cut Scene System",cutScene.css, typeof(CutSceneSystem), true);
		//draw bool options
		bool pauseGame = GUILayout.Toggle (cutScene.pauseGame,"Pause The Game",GUILayout.Width(300));
		cutScene.pauseGame = pauseGame;
		//draw buttons for adding and subtracting the cutscene sequence:
		GUILayout.BeginHorizontal ();
		if(GUILayout.Button("+Dialogo",GUILayout.Width(100))){
			Dialogue newDialogue = new Dialogue();
			cutScene.nodeList.Add(newDialogue);
			//EditorUtility.SetDirty(newDialogue);
		}
		if(GUILayout.Button("+Animation",GUILayout.Width(100))){
			cutScene.nodeList.Add(new AnimationNode());
		}
		GUILayout.EndHorizontal ();

		//show list
		grouping = false;
		foreach(CutSceneNodes nodeS in cutScene.nodeList){
			GUILayout.BeginVertical("box");

			if(nodeS is Dialogue){
				Dialogue node = (Dialogue)nodeS;
				if(GUILayout.Button("- Delete",GUILayout.Width(100))){
					cutScene.nodeList.Remove(node);
				}
				GUILayout.Label("<<Dialogue>>");
				float time = EditorGUILayout.FloatField("Letter Pause Time in seconds: ",node.letterPause);
				if(time >= 0f && time <= 10){
					node.letterPause = time;
				}
				if(!cutScene.pauseGame){
					node.target = (GameObject)EditorGUILayout.ObjectField ("Target: ",node.target, typeof(GameObject), true);
					node.timeToLive = EditorGUILayout.FloatField("Time To Live",node.timeToLive);
				}
				GUILayout.BeginHorizontal();
				node.characterImage = (Sprite)EditorGUILayout.ObjectField ("Icon: ",node.characterImage, typeof(Sprite), true);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Text: ");	
				node.text = EditorGUILayout.TextArea(((Dialogue)node).text,GUILayout.Width(300),GUILayout.Height(60));
				GUILayout.EndHorizontal();
				if(cutScene.nodeList.IndexOf(nodeS)+1 < cutScene.nodeList.Count){
					if(GUILayout.Button("Group With Next Node")){
						grouping = true;
						index1 = cutScene.nodeList.IndexOf(nodeS);
						index2 = index1+1;
					}
				}
			}else if(nodeS is AnimationNode){
				AnimationNode node = (AnimationNode)nodeS;

				if(GUILayout.Button("- Delete",GUILayout.Width(100))){
					cutScene.nodeList.Remove(node);
				}
				GUILayout.Label("<<Animation>>");
				node.animation = (Animation)EditorGUILayout.ObjectField ("Animation: ",node.animation, typeof(Animation), true);
				if(cutScene.nodeList.IndexOf(node)+1 < cutScene.nodeList.Count){
					if(GUILayout.Button("Group With Next Node")){
						grouping = true;
						index1 = cutScene.nodeList.IndexOf(node);
						index2 = index1+1;
					}
				}
			}else{
				CompositeCutSceneNode nodeC = (CompositeCutSceneNode)nodeS;
				if(nodeC.children.Count>0){
					for(int j=0;j<nodeC.children.Count;j++){
						CutSceneNodes nodeSC = nodeC.children[j];
						if(nodeSC is Dialogue){
							Dialogue nodeI = (Dialogue)nodeSC;
							if(GUILayout.Button("- Delete",GUILayout.Width(100))){
								nodeC.children.RemoveAt(j);
							}
							GUILayout.Label("<<Dialogue>>");
							float time = EditorGUILayout.FloatField("Letter Pause Time in seconds: ",nodeI.letterPause);
							if(time >= 0f && time <= 10){
								nodeI.letterPause = time;
							}
							if(!cutScene.pauseGame){
								nodeI.target = (GameObject)EditorGUILayout.ObjectField ("Target: ",nodeI.target, typeof(GameObject), true);
								nodeI.timeToLive = EditorGUILayout.FloatField("Time To Live",nodeI.timeToLive);
							}
							GUILayout.BeginHorizontal();
							nodeI.characterImage = (Sprite)EditorGUILayout.ObjectField ("Icon: ",nodeI.characterImage, typeof(Sprite), true);
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
							GUILayout.Label("Text: ");	
							nodeI.text = EditorGUILayout.TextArea(((Dialogue)nodeI).text,GUILayout.Width(300),GUILayout.Height(60));
							GUILayout.EndHorizontal();
						}else if(nodeSC is AnimationNode){
							AnimationNode nodeSA = (AnimationNode)nodeSC;
							
							if(GUILayout.Button("- Delete",GUILayout.Width(100))){
								nodeC.children.RemoveAt(j);
							}
							GUILayout.Label("<<Animation>>");
							nodeSA.animation = (Animation)EditorGUILayout.ObjectField ("Animation: ",nodeSA.animation, typeof(Animation), true);

						}
						if(j+1 < nodeC.children.Count){
							if(GUILayout.Button("-- Break Group Here --")){
								CompositeCutSceneNode newComp = new CompositeCutSceneNode();
								newComp.children = nodeC.children.GetRange(j+1,nodeC.children.Count - 1 - j);
								nodeC.children.RemoveRange(j+1,nodeC.children.Count - 1 - j);
								int index = cutScene.nodeList.IndexOf(nodeS);
								cutScene.nodeList.Insert(index+1,newComp);
							}
						}
					}
				}

				if(cutScene.nodeList.IndexOf(nodeS)+1 < cutScene.nodeList.Count){
					if(GUILayout.Button("Group With Next Node")){
						grouping = true;
						index1 = cutScene.nodeList.IndexOf(nodeS);
						index2 = index1+1;
					}
				}

			}
			//nodeS.playWithNext = EditorGUILayout.Toggle("Play With Next: ",nodeS.playWithNext);
			GUILayout.EndVertical();
			EditorUtility.SetDirty(cutScene);
		}

		if(grouping){
			CutSceneNodes g1  = cutScene.nodeList[index1];
			CutSceneNodes g2 = cutScene.nodeList[index2];
			if(g1 is CompositeCutSceneNode){
				if(g2 is CompositeCutSceneNode){
					((CompositeCutSceneNode)g1).children.AddRange(((CompositeCutSceneNode)g2).children);
					cutScene.nodeList.Remove(g2);
				}else{
					((CompositeCutSceneNode)g1).children.Add(g2);
					cutScene.nodeList.Remove(g2);
				}
			}else{
				if(g2 is CompositeCutSceneNode){
					((CompositeCutSceneNode)g2).children.Insert(0,g1);
					cutScene.nodeList.Remove(g1);
				}else{
					CompositeCutSceneNode composite = new CompositeCutSceneNode();
					composite.children.Add(g1);
					composite.children.Add(g2);
					cutScene.nodeList.Insert(index2+1,composite);
					cutScene.nodeList.Remove(g1);
					cutScene.nodeList.Remove(g2);
				}
			}
			grouping = false;
		}

		foreach(CutSceneNodes nodeS in cutScene.nodeList){
			if(nodeS is CompositeCutSceneNode){
				if(((CompositeCutSceneNode)nodeS).children.Count < 1){
					cutScene.nodeList.Remove(nodeS);
				}else if(((CompositeCutSceneNode)nodeS).children.Count == 1){
					int index = cutScene.nodeList.IndexOf(nodeS);
					cutScene.nodeList.Insert(index+1,((CompositeCutSceneNode)nodeS).children[0]);
					cutScene.nodeList.Remove(nodeS);
				}
			}
		}


	}

	[MenuItem("Cut Scene System/Add CutScene Component to gameObject")]
	static void addCutsceneComponentToGameObject(){
		GameObject obj = (GameObject)Selection.activeGameObject;
		if(obj!=null){
			Undo.AddComponent(obj,typeof(CutScene));
		}
	}
	
	// Validate the menu item defined by the function above.
	// The menu item will be disabled if this function returns false.
	[MenuItem ("Cut Scene System/Add CutScene Component to gameObject", true)]
	static bool ValidateaddCutsceneComponentToGameObject () {
		// Return false if no transform is selected.
		return (Selection.activeGameObject != null);
	}

	[MenuItem("Cut Scene System/Add Cut Scene System To Scene")]
	static void CreateCutSceneSytem (MenuCommand command) {
		if(GameObject.Find("CutSceneSystem") == null){
			GameObject sys= Instantiate(Resources.Load("CutSceneSystem", typeof(GameObject))) as GameObject;
			sys.name = "CutSceneSystem";
			Undo.RegisterCreatedObjectUndo(sys, "Create " + sys.name);
		}
	}

	[MenuItem("Cut Scene System/Add Cut Scene System To Scene",true)]
	static bool ValidateCreateCutSceneSytem (MenuCommand command) {
		if(GameObject.Find("CutSceneSystem") == null){
			return true;
		}
		return false;
	}
}*/

/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
*/
#endif