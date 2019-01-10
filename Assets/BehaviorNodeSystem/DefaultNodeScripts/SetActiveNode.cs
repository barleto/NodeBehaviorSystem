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
    public class SetActiveNode : BehaviorNode
    {
        [SerializeField]
        public GameObject gameObj;
        public bool active = false;

#if UNITY_EDITOR
        public override void createUIDescription(BehaviorListHolder BehaviorList, SerializedObject serializedObject)
        {
            SetActiveNode node = this;
            GUILayout.Label("<<SetActiveNode>>");
            gameObj = (GameObject)EditorGUILayout.ObjectField("Object: ", gameObj, typeof(GameObject), true);
            active = EditorGUILayout.Toggle("Active: ", active);


        }
#endif

        public override void OnStart()
        {
            gameObj.SetActive(active);
            EndNodeExecution();
        }

        public override void OnUpdate()
        {

        }

        public override void OnEnd()
        {

        }


    }
}