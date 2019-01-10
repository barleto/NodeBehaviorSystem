using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BehaviorNodePlugin
{
    [System.Serializable]
    public class BehaviorNodesSystem : MonoBehaviour
    {

        public bool startPlaying;
        public BehaviorListHolder initialBehaviorList;
        [SerializeField] private BehaviorListHolder _currentBehaviorList;
        public UnityEvent onBehaviorListStart;
        public UnityEvent onBehaviorListEnd;

        private Coroutine _playingBehaviorNodesCoroutine;
        private BehaviorNode _currentNode = null;

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

        public void PlayScene(BehaviorListHolder newScene)
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
            foreach (BehaviorNode node in _currentBehaviorList.nodeListAsset.list)
            {
                _currentNode = node;
                node.behaviourList = _currentBehaviorList;
                node.init();
                node.OnStart();
                while (!node.HasExecutionEnded())
                {
                    node.OnUpdate();
                    yield return null;
                }
                node.OnEnd();
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
                _currentNode.OnEnd();
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