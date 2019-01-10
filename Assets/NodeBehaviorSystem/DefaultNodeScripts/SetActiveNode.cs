using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

namespace BehaviorNode
{
    [System.Serializable]
    public class SetActiveNode : BehaviorListNode
    {
        [SerializeField]
        public GameObject gameObj;
        public bool active = false;

#if UNITY_EDITOR
        public override void createUIDescription(BehaviorList BehaviorList, SerializedObject serializedObject)
        {
            SetActiveNode node = this;
            GUILayout.Label("<<SetActiveNode>>");
            gameObj = (GameObject)EditorGUILayout.ObjectField("Object: ", gameObj, typeof(GameObject), true);
            active = EditorGUILayout.Toggle("Active: ", active);


        }
#endif

        public override void start()
        {
            gameObj.SetActive(active);
            EndNodeExecution();
        }

        public override void update()
        {

        }

        public override void end()
        {

        }


    }
}