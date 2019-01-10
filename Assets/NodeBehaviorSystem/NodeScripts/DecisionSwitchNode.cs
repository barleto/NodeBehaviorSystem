using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

[System.Serializable]
public class DecisionSwitchNode : CutSceneNode {

	[SerializeField]
	public enum SwitchState{On,Off};
	[SerializeField]
	private int selectedIndex;
	[SerializeField]
	private List<string> listOfSwitches = new List<string>(); 
	[SerializeField]
	private int indexOfSwitch;
	[SerializeField]
	private CutScene cutSceneToGo;

	//inGame variables
	[SerializeField]
	private GameSwitch decisionSwitch = null;
	[SerializeField]
	private SwitchState state = SwitchState.Off;


	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		//display
		GUILayout.Label("<<Decision OnSwitch Node>>");
		//switch system here:
		listOfSwitches.Clear ();
		listOfSwitches.Add ("None");
		foreach(GameSwitch gameSwitch in cutScene.cutSceneSystem.switchVariables){
			listOfSwitches.Add(gameSwitch.name);
		}

		indexOfSwitch = EditorGUILayout.Popup ("Switch to check: ",indexOfSwitch,listOfSwitches.ToArray());

		if (cutScene.cutSceneSystem.switchVariables.Count > 0 && (indexOfSwitch -1) >= 0) {
			decisionSwitch = cutScene.cutSceneSystem.switchVariables [indexOfSwitch - 1];
		} else {
			decisionSwitch = null;
		}

		//Now, for the inpector variables
		cutSceneToGo = (CutScene)EditorGUILayout.ObjectField ("Cut Scene To Jump if switch's true: ",cutSceneToGo, typeof(CutScene), true);
		if (cutSceneToGo == null) {
			EditorGUILayout.HelpBox ("if the switch is false the cutscene will stop in this node and will not execute the next nodes. If true, continues normally executing.", MessageType.Info);
		} else {
			EditorGUILayout.HelpBox ("If the switch is false, the execution of the cutscene continues normally. If the switch is true, then the target cutscene is going to be played next.", MessageType.Info);
		}
	}
#endif

	public override void start(){
		EndNodeExecution();
		if (cutSceneToGo != null) {
			if(decisionSwitch.value == true){
				cutScene.cutSceneSystem.PlayScene(cutSceneToGo);
			}
		} else {
			if(decisionSwitch.value == false){
				cutScene.cutSceneSystem.StopScene();
			}
		}
	}
	
	public override void update(){
	}
	
	public override void end(){

	}


}
