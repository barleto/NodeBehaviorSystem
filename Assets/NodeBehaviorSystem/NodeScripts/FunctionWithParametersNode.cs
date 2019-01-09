using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

[System.Serializable]
public class FunctionWithParametersNode : CutSceneNodes {
	[SerializeField]
	public EventsForNode eventCallee = null;
	[SerializeField]
	public string functionName;
	
	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		FunctionWithParametersNode node = this;
		GUILayout.Label("<<Execute Function With Parameters>>");
		eventCallee = (EventsForNode)EditorGUILayout.ObjectField ("EventsToTrigger: ",eventCallee, typeof(EventsForNode),true);


	}
	#endif
	
	public override void start(){
		this.eventCallee.OnNodeExecution.Invoke ();
		hasExecutionEnded = true;
	}
	
	public override  void update(){
		
	}
	
	public override  void end(){
		
	}



}


