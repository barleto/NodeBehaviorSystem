using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

[System.Serializable]
public class PlayAudio : CutSceneNode {
	[SerializeField]
	public GameObject objectSource;
	public AudioClip audio;

	private AudioSource audioSource;

	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		GUILayout.Label("<<Audio>>");
		objectSource = (GameObject)EditorGUILayout.ObjectField ("Object with audio source: ",objectSource, typeof(GameObject),true);
		audio = (AudioClip)EditorGUILayout.ObjectField ("Audio: ", audio, typeof(AudioClip), true);
	}
	#endif
	
	public override void start(){
		audioSource = objectSource.GetComponent<AudioSource>();
		audioSource.PlayOneShot(audio, 1F);
	}
	
	public override  void update(){
		if(!audioSource.isPlaying){
			EndNodeExecution();
		}
	}
	
	public override  void end(){
	}
	
	
}
