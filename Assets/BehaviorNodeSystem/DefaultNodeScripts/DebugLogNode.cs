using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviorNodePlugin
{
    [CreateAssetMenu()]
    public class DebugLogNode : BehaviorNode
    {
        [SerializeField]
        public string text = "";

#if UNITY_EDITOR
        public override void createUIDescription(BehaviorListHolder BehaviorList, SerializedObject serializedObject)
        {
            GUILayout.Label("<<Debug Log>>");
            text = EditorGUILayout.TextArea(text);
        }
#endif

        public override void OnStart()
        {

        }

        public override void OnUpdate()
        {
            Debug.Log(text);
            EndNodeExecution();
        }

        public override void OnEnd()
        {

        }
    }
}