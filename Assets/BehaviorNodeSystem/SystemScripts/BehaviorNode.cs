using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviorNodePlugin
{
    [System.Serializable]
    public class BehaviorNode : ScriptableObject
    {

        private bool _hasExecutionEnded = false;
        [HideInInspector]
        public BehaviorListHolder behaviourList;

#if UNITY_EDITOR
        /*In this function, you define what will appear in the UI of the BehaviorList.
        Just populate with GUILayout funcitons*/
        virtual public void createUIDescription(BehaviorListHolder behaviorList, SerializedObject serializedObject)
        {

        }
#endif

        public void init(){
            _hasExecutionEnded = false;
        }

        //executed once to initialize the node
        virtual public void OnStart()
        {

        }

        //executed each frame
        virtual public void OnUpdate()
        {
        }

        //executed when the node finished executing. Finalize thng if you want to.
        virtual public void OnEnd()
        {

        }

        public void EndNodeExecution()
        {
            _hasExecutionEnded = true;
        }

        public bool HasExecutionEnded()
        {
            return _hasExecutionEnded;
        }

        public virtual bool HasCustomInspector(){
            return false;
        }
    }
}

