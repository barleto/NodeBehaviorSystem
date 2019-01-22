using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

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
                PlayList(initialBehaviorList);
            }
        }

        public void PlayList(BehaviorListHolder newBehaviorListHolder)
        {
            EndCurrentBehaviorList();
            _currentBehaviorList = newBehaviorListHolder;
            onBehaviorListStart.Invoke();
            newBehaviorListHolder.behaviorNodeSystem = this;
            _playingBehaviorNodesCoroutine = StartCoroutine(PlayBehaviorNodesCoroutine());
        }

        public void Stop()
        {
            EndCurrentBehaviorList();
        }

        private IEnumerator PlayBehaviorNodesCoroutine()
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

        internal void PlayList(object listHolder)
        {
            throw new NotImplementedException();
        }
    }
}