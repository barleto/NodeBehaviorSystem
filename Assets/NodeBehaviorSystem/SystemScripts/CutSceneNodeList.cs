using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CutSceneNodeList : ScriptableObject {

	[SerializeField]
	public List<CutSceneNodes> nodeList;

}

