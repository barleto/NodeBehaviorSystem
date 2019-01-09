using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CutScene : MonoBehaviour {

	[HideInInspector]
	public GameSwitch gameSwitch;

	public List<CutSceneNodes> nodeList;

	public bool pauseGame = true;

	public CutSceneSystem css;

	[HideInInspector]
	public int indexOfSwitch = 0;

	[HideInInspector]
	public CutSceneNodeList csnl;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
