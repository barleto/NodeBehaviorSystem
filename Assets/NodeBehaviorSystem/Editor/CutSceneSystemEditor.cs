using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(CutSceneSystem))]
public class CutSceneSystemEditor : Editor {

	public string switchNameField = "";

	public override void OnInspectorGUI(){

		DrawDefaultInspector ();
		CutSceneSystem css = (CutSceneSystem)target;
		EditorGUILayout.BeginVertical ("box");
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("New Switch:");
		switchNameField = EditorGUILayout.TextField (switchNameField);
		if(GUILayout.Button("Add Switch",GUILayout.Width(100)) && switchNameField != ""){
			if(css.SwitchVariables == null){
				css.SwitchVariables = new List<GameSwitch>();
			}
			if(!checkNameInSwitchList(switchNameField,css)){
				GameSwitch gs = GameSwitch.CreateInstance<GameSwitch>();
				gs.name = switchNameField;
				gs.value = false;
				css.SwitchVariables.Add(gs);
				switchNameField = "";
			}else{
				Debug.Log("Switch with name "+switchNameField+" already exists!");
			}
		}
		EditorGUILayout.EndHorizontal ();

		foreach(GameSwitch gameSwitch in css.SwitchVariables){
			EditorGUILayout.BeginHorizontal ();
			if(GUILayout.Button("-",GUILayout.Width(25))){
					css.SwitchVariables.Remove(gameSwitch);
			}
				EditorGUILayout.LabelField (gameSwitch.name+" , "+gameSwitch.value);
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.EndVertical ();
	}

	bool checkNameInSwitchList(string name, CutSceneSystem css){
		foreach(GameSwitch gameSwitch in css.SwitchVariables){
			if(gameSwitch.name == name){
				return true;
			}
		}
		return false;
	}
	

}
#endif