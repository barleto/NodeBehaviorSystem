using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AnimationNode : CutSceneNodes {
	public Animation animation = null;

	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		AnimationNode node = this;
		GUILayout.Label("<<Animation>>");
		node.animation = (Animation)EditorGUILayout.ObjectField ("Animation: ",node.animation, typeof(Animation), true);
	}
#endif

	public override void start(){
		animation.Play ();
	}
	
	public override  void update(){
		if(!(animation.isPlaying)){
			hasExecutionEnded = true;
		}
	}
	
	public override  void end(){
		animation.Stop ();
	}
}
