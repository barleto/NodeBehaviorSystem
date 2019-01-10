using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BehaviorNodePlugin
{
    [System.Serializable]
    public class BehaviorNodesList : ScriptableObject
    {

        public List<BehaviorNode> list = new List<BehaviorNode>();

    }
}
