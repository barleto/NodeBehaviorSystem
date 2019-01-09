using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class DialogueNonStop : CutSceneNodes {
	public string text;
	public float timeToLive = 1.0f;
	[Range(0.1f,5f)]
	public float letterPause = 0.1f;
	public Canvas canvas;
	public Text chatBox;

	[SerializeField]
	private bool hasFinishedWritingText = false;
	[SerializeField]
	private float countTime = 0;
	[SerializeField]
	private Coroutine textRoutine;

	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		DialogueNonStop node = this;
		GUILayout.Label("<<Dialogue>>");
		float time = EditorGUILayout.Slider("Show Speed",node.letterPause,0,1f);
		if(time >= 0f && time <= 10){
			node.letterPause = time;
		}
		Canvas last = canvas;
		node.canvas = (Canvas)EditorGUILayout.ObjectField ("Chat Box Canvas: ",node.canvas, typeof(Canvas), true);
		if(node.canvas != last && canvas != null){
			bool isActive = node.canvas.gameObject.activeSelf;
			node.canvas.gameObject.SetActive(true);
			node.chatBox = canvas.GetComponentInChildren<Text>();
			node.canvas.gameObject.SetActive(isActive);
		}
		node.chatBox = (Text)EditorGUILayout.ObjectField ("UIText inside above Canvas: ",node.chatBox, typeof(Text), true);
		node.timeToLive = EditorGUILayout.FloatField("Time To Live",node.timeToLive);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Text: ");	
		node.text = EditorGUILayout.TextArea(node.text,GUILayout.Width(300),GUILayout.Height(60));
		GUILayout.EndHorizontal();
	}
#endif

	public override void start(){
		canvas.gameObject.SetActive (true);
		chatBox.text = "";
		textRoutine = cutScene.StartCoroutine (showText ());
		countTime = 0;
	}
	
	public override  void update(){
		countTime += Time.deltaTime;
		if(countTime >= timeToLive){
			hasExecutionEnded = true;
		}
	}
	
	public override  void end(){
		canvas.gameObject.SetActive (false);
		cutScene.StopCoroutine (textRoutine);
	}

	public override void tapAtScreen ()
	{

	}

	public IEnumerator showText () {
		foreach (char letter in text.ToCharArray()) {
			chatBox.text += letter;
			yield return new WaitForSeconds (letterPause);
		}
		hasFinishedWritingText = true;
	}
}
