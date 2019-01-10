using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CompositeCutSceneNode : CutSceneNode {
	public List<CutSceneNode> children = new List<CutSceneNode>();

	public override void start(){
		foreach(CutSceneNode node in children){
			node.start();
		}
	}
	
	public override  void update(){
		bool hasAllFinished = true;
		foreach(CutSceneNode node in children){
			if(HasExecutionEnded() == false){
				hasAllFinished = false;
			}
			node.update();
		}
		if(hasAllFinished){
            EndNodeExecution();
        }
	}

	public override  void end(){
		foreach(CutSceneNode node in children){
			node.end();
		}
	}

	public override void tapAtScreen(){
		foreach(CutSceneNode node in children){
			node.tapAtScreen();
		}
	}
}
