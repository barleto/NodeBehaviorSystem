using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

namespace BehaviorNodePlugin
{
    [System.Serializable]
    public class WaitForTimeNode : BehaviorNode
    {
        [SerializeField]
        public float timeToWait = 1;
        private float timePassed;
#if UNITY_EDITOR
        public override void createUIDescription(BehaviorListHolder BehaviorList, SerializedObject serializedObject)
        {
            GUILayout.Label("<<Time to wait>>");
            timeToWait = EditorGUILayout.FloatField("Time to wait: ", timeToWait);
        }
#endif

        public override void OnStart()
        {
            timePassed = 0;
        }

        public override void OnUpdate()
        {
            timePassed += Time.deltaTime;
            if (timePassed >= timeToWait)
            {
                EndNodeExecution();
            }
        }

        public override void OnEnd()
        {

        }


    }
}