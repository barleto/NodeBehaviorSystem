using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

[System.Serializable]
public class SetSwitchValueNode : CutSceneNode {

	[SerializeField]
	public enum SwitchState{On,Off};
	[SerializeField]
	private int selectedIndex;
	[SerializeField]
	private List<string> switchNames = new List<string>();

	//inGame variables
	[SerializeField]
	private GameSwitch targetSwitch;
	[SerializeField]
	private SwitchState state = SwitchState.Off;


	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		int i = 0;

		//display
		GUILayout.Label("<<Change Switch Value>>");
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Set ");
		switchNames.Clear ();
		foreach(GameSwitch gameSwitch in cutScene.cutSceneSystem.switchVariables){
			switchNames.Add(gameSwitch.name);
		}


		selectedIndex = (int)EditorGUILayout.Popup (selectedIndex, switchNames.ToArray());
		if (cutScene.cutSceneSystem.switchVariables.Count > 0) {
			targetSwitch = cutScene.cutSceneSystem.switchVariables[selectedIndex];
		}

		GUILayout.Label (" to ");
		state = (SwitchState)EditorGUILayout.EnumPopup ("", state);
		EditorGUILayout.EndHorizontal ();
		//eventCallee = (MonoBehaviour)EditorGUILayout.ObjectField ("Object: ",eventCallee, typeof(MonoBehaviour),true);
		//functionName = EditorGUILayout.TextField ("FuncitonName: ",functionName);
	}
#endif

	public override void start(){
		if(targetSwitch != null){
			if (state == SwitchState.On) {
				targetSwitch.value = true;
			} else {
				targetSwitch.value = false;
			}
		}
		EndNodeExecution();
	}
	
	public override  void update(){
	}
	
	public override  void end(){

	}


}
