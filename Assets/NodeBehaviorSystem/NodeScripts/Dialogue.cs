using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class Dialogue : CutSceneNodes {
	public Sprite characterImage;
	public string text;
	[Range(0.1f,5f)]
	public float letterPause = 0.0f;
	public bool dontWaitForPlayerTap = false;
	public bool dontLetPlayerTap = false;
	
	private Text textBox;
	private bool hasFinishedWritingText = false;
	private Coroutine textRoutine;

	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		Dialogue node = this;
		GUILayout.Label("<<Dialogue>>");
		float time = EditorGUILayout.Slider("Show Speed",node.letterPause,0,1f);
		if(time >= 0f && time <= 10){
			node.letterPause = time;
		}
		GUILayout.BeginHorizontal();
		node.characterImage = (Sprite)EditorGUILayout.ObjectField ("Icon: ",node.characterImage, typeof(Sprite), true);
		GUILayout.EndHorizontal();
		dontWaitForPlayerTap = EditorGUILayout.Toggle("Don't wait for player tap? ", dontWaitForPlayerTap);
		dontLetPlayerTap = EditorGUILayout.Toggle("Don't let player tap to finish?", dontLetPlayerTap);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Text: ");	
		node.text = EditorGUILayout.TextArea(((Dialogue)node).text,GUILayout.Width(300),GUILayout.Height(60));
		GUILayout.EndHorizontal();
	}
#endif

	public override void start(){
		base.start ();
		cutScene.css.talkImage.sprite = characterImage;
		textBox = cutScene.css.textBox;
		textBox.text = "";
		hasFinishedWritingText = false;
		cutScene.css.toggleUIVisibility (true);
		textRoutine = cutScene.StartCoroutine (showText ());
	}
	
	public override  void update(){
		if(dontWaitForPlayerTap && hasFinishedWritingText){
			hasExecutionEnded = true;
		}
	}
	
	public override  void end(){
		base.end ();
		if(textRoutine!=null){
			cutScene.StopCoroutine (textRoutine);
		}
		cutScene.css.toggleUIVisibility (false);
		hasFinishedWritingText = false;
	}

	public override void tapAtScreen ()
	{
		if(hasFinishedWritingText){
			hasExecutionEnded = true;
		} else if(!dontLetPlayerTap){
			finishShowText();
		}
	}

	public IEnumerator showText () {
		foreach (char letter in text.ToCharArray()) {
			textBox.text += letter;
			yield return new WaitForSeconds (letterPause);
		}
		hasFinishedWritingText = true;
	}

	public void finishShowText(){
		cutScene.StopCoroutine(textRoutine);
		textBox.text = text;
		hasFinishedWritingText = true;
	}
}
