using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu()]
public class DebugLogNode : CutSceneNode {
    [SerializeField]
    public string text = "";

#if UNITY_EDITOR
    public override void createUIDescription(CutScene cutScene, SerializedObject serializedObject)
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
