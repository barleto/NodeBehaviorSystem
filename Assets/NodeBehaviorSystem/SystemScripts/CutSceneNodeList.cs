using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CutSceneNodeList : ScriptableObject {
    [HideInInspector]
	public List<CutSceneNode> list = new List<CutSceneNode>();

}

