using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class CutSceneSystem : MonoBehaviour {

	public bool startPlaying;
	public CutScene initialCutScene;
    [SerializeField] private CutScene _currentCutScene;
	public UnityEvent onCutSceneStart;
	public UnityEvent onCutSceneEnd;
    
    private Coroutine _playingBehaviorNodesCoroutine;
    private CutSceneNode _currentNode = null;

	//switch sytem variables
	[HideInInspector]
	public List<GameSwitch> switchVariables;
	
	void Start () {
		if(startPlaying && initialCutScene != null){
			PlayScene (initialCutScene);
		}
	}
	
	public void PlayScene(CutScene newScene){
		EndCurrentCutscene ();
		_currentCutScene = newScene;
		_playingBehaviorNodesCoroutine = StartCoroutine (PlayBehaviorNodesCoroutine());
	}
	
	public void StopScene(){
		EndCurrentCutscene ();
	}
	
	public IEnumerator PlayBehaviorNodesCoroutine(){
        foreach (CutSceneNode node in _currentCutScene.nodeListAsset.list)
        {
            _currentNode = node;
            node.start();
            while (!node.HasExecutionEnded())
            {
                node.update();
                yield return null;
            }
            node.end();
        }
	}

	private void EndCurrentCutscene(){
        if (_currentCutScene == null)
        {
            return;
        }
        if (_currentNode!=null){
			_currentNode.EndNodeExecution();
			_currentNode.end();
		}
		if(_playingBehaviorNodesCoroutine != null){
			StopCoroutine(_playingBehaviorNodesCoroutine);
		}
		_currentCutScene = null;
	}
}