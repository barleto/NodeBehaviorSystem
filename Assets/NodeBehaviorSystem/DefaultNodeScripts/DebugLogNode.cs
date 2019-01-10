using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviorNode
{
    [CreateAssetMenu()]
    public class DebugLogNode : BehaviorListNode
    {
        [SerializeField]
        public string text = "";

#if UNITY_EDITOR
        public override void createUIDescription(BehaviorList BehaviorList, SerializedObject serializedObject)
        {
            GUILayout.Label("<<Debug Log>>");
            text = EditorGUILayout.TextArea(text);
        }
#endif

        public override void start()
        {

        }

        public override void update()
        {
            Debug.Log(text);
            EndNodeExecution();
        }

        public override void end()
        {

        }
    }
}