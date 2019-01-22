using System.Collections;
using System.Collections.Generic;
using BehaviorNodePlugin;
using UnityEngine;

public class PlayListNode : BehaviorNode {
    public BehaviorNodesList newList;

    public override void OnStart()
    {
        behaviourList.nodeListAsset = newList;
        HasExecutionEnded();
        behaviourList.behaviorNodeSystem.PlayList(behaviourList);
    }

}