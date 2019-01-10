using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorNodePlugin
{
    public class BehaviorNodeSystemConfiguration : ScriptableObject
    {

        [SerializeField] private string _pathToSaveNodeLists = "Assets/NodeBehaviorSystem/NodesList/";
        [SerializeField] private GameObject _behaviorSystemPrefab;

        public string GetPathToSaveNodeLists()
        {
            return _pathToSaveNodeLists;
        }

        public GameObject GetSystemPrefab()
        {
            return _behaviorSystemPrefab;
        }
    }
}