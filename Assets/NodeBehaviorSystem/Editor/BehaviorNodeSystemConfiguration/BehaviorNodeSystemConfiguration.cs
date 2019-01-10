using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorNodeSystemConfiguration : ScriptableObject {

    [SerializeField] private string _pathToSaveNodeLists = "Assets/NodeBehaviorSystem/NodesList/";

    public string GetPathToSaveNodeLists()
    {
        return  _pathToSaveNodeLists;
    }
}
