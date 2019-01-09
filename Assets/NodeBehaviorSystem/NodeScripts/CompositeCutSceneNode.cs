using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CompositeCutSceneNode : CutSceneNodes {
	public List<CutSceneNodes> children = new List<CutSceneNodes>();

	public override void start(){
		hasExecutionEnded = false;
		foreach(CutSceneNodes node in children){
			node.start();
		}
	}
	
	public override  void update(){
		bool hasAllFinished = true;
		foreach(CutSceneNodes node in children){
			if(node.hasExecutionEnded == false){
				hasAllFinished = false;
			}
			node.update();
		}
		if(hasAllFinished){
			hasExecutionEnded = true;
		}
	}

	public override  void end(){
		foreach(CutSceneNodes node in children){
			node.end();
		}
	}

	public override void tapAtScreen(){
		foreach(CutSceneNodes node in children){
			node.tapAtScreen();
		}
	}
}
