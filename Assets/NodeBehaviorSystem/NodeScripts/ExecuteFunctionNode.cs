using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

[System.Serializable]
public class ExecuteFunctionNode : CutSceneNode {
	[SerializeField]
	public MonoBehaviour eventCallee = null;
	[SerializeField]
	public string functionName;

	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		ExecuteFunctionNode node = this;
		GUILayout.Label("<<Execute Function>>");
		eventCallee = (MonoBehaviour)EditorGUILayout.ObjectField ("Object: ",eventCallee, typeof(MonoBehaviour),true);
		functionName = EditorGUILayout.TextField ("FuncitonName: ",functionName);
	}
#endif

	public override void start(){
		this.eventCallee.Invoke (functionName,0);
		EndNodeExecution();
	}
	
	public override  void update(){

	}
	
	public override  void end(){

	}


}
