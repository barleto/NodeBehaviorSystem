using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BehaviorNode
{
    [System.Serializable]
    public class BehaviorNodesList : ScriptableObject
    {

        public List<BehaviorListNode> list = new List<BehaviorListNode>();

    }
}
