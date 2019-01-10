using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class CutSceneSystem : MonoBehaviour {

	public bool startPlaying;
	public CutScene initialCutScene;
	public UnityEvent onCutSceneStart;
	public UnityEvent onCutSceneEnd;


    [SerializeField] private CutScene _currentCutScene;
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
            node.start();
            while (!_currentNode.HasExecutionEnded())
            {
                _currentNode.update();
                yield return null;
            }
            _currentNode.end();
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