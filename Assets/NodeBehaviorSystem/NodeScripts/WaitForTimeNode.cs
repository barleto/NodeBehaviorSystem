using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

[System.Serializable]
public class WaitForTimeNode : CutSceneNodes {
	[SerializeField]
	public float timeToWait = 1;
	private float timePassed;
	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		GUILayout.Label("<<Time to wait>>");
		timeToWait = EditorGUILayout.FloatField("Time to wait: ", timeToWait);
	}
	#endif
	
	public override void start(){
		timePassed = 0;
	}
	
	public override  void update(){
		timePassed += Time.deltaTime;
		if(timePassed >= timeToWait){
			hasExecutionEnded = true;
		}
	}
	
	public override  void end(){
		
	}
	
	
}
