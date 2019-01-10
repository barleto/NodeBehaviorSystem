using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BehaviorNode
{
    [System.Serializable]
    public class BehaviorNodesSystem : MonoBehaviour
    {

        public bool startPlaying;
        public BehaviorList initialBehaviorList;
        [SerializeField] private BehaviorList _currentBehaviorList;
        public UnityEvent onBehaviorListStart;
        public UnityEvent onBehaviorListEnd;

        private Coroutine _playingBehaviorNodesCoroutine;
        private BehaviorListNode _currentNode = null;

        //switch sytem variables
        [HideInInspector]
        public List<GameSwitch> switchVariables;

        void Start()
        {
            if (startPlaying && initialBehaviorList != null)
            {
                PlayScene(initialBehaviorList);
            }
        }

        public void PlayScene(BehaviorList newScene)
        {
            EndCurrentBehaviorList();
            _currentBehaviorList = newScene;
            onBehaviorListStart.Invoke();
            _playingBehaviorNodesCoroutine = StartCoroutine(PlayBehaviorNodesCoroutine());
        }

        public void StopScene()
        {
            EndCurrentBehaviorList();
        }

        public IEnumerator PlayBehaviorNodesCoroutine()
        {
            foreach (BehaviorListNode node in _currentBehaviorList.nodeListAsset.list)
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
            onBehaviorListEnd.Invoke();
        }

        private void EndCurrentBehaviorList()
        {
            if (_currentBehaviorList == null)
            {
                return;
            }
            if (_currentNode != null)
            {
                _currentNode.EndNodeExecution();
                _currentNode.end();
            }
            if (_playingBehaviorNodesCoroutine != null)
            {
                StopCoroutine(_playingBehaviorNodesCoroutine);
            }
            _currentBehaviorList = null;
            onBehaviorListEnd.Invoke();
        }
    }
}