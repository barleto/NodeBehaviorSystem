using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FadeOutSpriteNode : CutSceneNodes {

	public SpriteRenderer spriteRenderer = null;
	public float finalAlpha = 0;
	public float timeToFade = 1;

	private float speed;
	
	#if UNITY_EDITOR
	public override void createUIDescription(CutScene cutScene,SerializedObject serializedObject){
		FadeOutSpriteNode node = this;
		GUILayout.Label("<<Fade Out Sprite>>");
		this.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField ("Sprite Renderer: ",this.spriteRenderer, typeof(SpriteRenderer), true);
		//this.finalAlpha = EditorGUILayout.FloatField ("Final Alpha:",this.finalAlpha);
		this.timeToFade = EditorGUILayout.FloatField ("Time to Fade:",this.timeToFade);
		if(timeToFade<0){
			timeToFade = 0;
		}
	}
	#endif
	
	public override void start(){
		this.speed = 1 / timeToFade;
	}
	
	public override  void update(){

		if (spriteRenderer.color.a <= 0) {
			hasExecutionEnded = true;
		} else {
			spriteRenderer.color = new Color(spriteRenderer.color.r,spriteRenderer.color.g,spriteRenderer.color.b, spriteRenderer.color.a - speed * Time.deltaTime);
		}
	}
	
	public override  void end(){
	}
}