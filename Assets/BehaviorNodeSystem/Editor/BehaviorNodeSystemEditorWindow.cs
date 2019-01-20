using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using BehaviorNodePlugin;
using UnityEngine;
using System;
using System.Linq;

public class BehaviorNodeSystemEditorWindow : EditorWindow {

    BehaviorNodesList _currentNodesList;
    List<WindowListItem> windows = new List<WindowListItem>();

    [MenuItem("Window/BehaviorNodeList")]
    static void ShowWindow()
    {
        var window = (BehaviorNodeSystemEditorWindow)EditorWindow.GetWindow(typeof(BehaviorNodeSystemEditorWindow));
        window.Show();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeObject is BehaviorNodesList)
        {
            _currentNodesList = Selection.activeObject as BehaviorNodesList;
            CreateWindowsListFromBehaviorsNodeList();
        }
        Repaint();
    }

    private void CreateWindowsListFromBehaviorsNodeList()
    {
        windows.Clear();
        List<BehaviorNode> list = _currentNodesList.list;
        for (int i = 0; i < list.Count(); i++)
        {
            var item = new WindowListItem(i,list[i].GetType().Name, new Rect(0,0,250,0), list[i]);
            windows.Add(item);
        }
    }


    void OnGUI()
    {
        if (_currentNodesList == null)
        {
            GUILayout.Label("There is no BehaviorNodeList selected.");
            return;
        }

        if (windows.Count() == 0)
        {
            GUILayout.Label("The list is empty.");
            return;
        }

        BeginWindows();
        for (int i = 0; i < windows.Count(); i++)
        {
            var item = windows[i];
            item.position = GUILayout.Window(item.id, item.position, DrawNodeWindow, item.name);
        }

        EndWindows();
    }

    void DrawNodeWindow(int id)
    {
        WindowListItem item = windows[id];
        BehaviorNode node = windows[id].node;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("×", GUILayout.Width(30)))
        {

        }
        if (GUILayout.Button("▲", GUILayout.Width(30)))
        {

        }
        if (GUILayout.Button("▼", GUILayout.Width(30)))
        {


        }
        if (GUILayout.Button(new GUIContent("T", "Send node to bottom"), GUILayout.Width(30)))
        {

        }
        if (GUILayout.Button(new GUIContent("B", "Send node to bottom"), GUILayout.Width(30)))
        {

        }
        GUILayout.EndHorizontal();
        EditorGUI.BeginChangeCheck();
        Editor.CreateEditor(node).OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(node);
        }

    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }

    protected class WindowListItem
    {
        public int id;
        public string name;
        public Rect position;
        public BehaviorNode node;

        public WindowListItem(int id, string name, Rect position, BehaviorNode node)
        {
            this.id = id;
            this.name = name;
            this.position = position;
            this.node = node;
        }
    }

    protected class NodeLayouter
    {
        public Rect accumulated = new Rect(0,0,0,0);

        public void UpdateEnclosingRect()
        {
            var rect = GUILayoutUtility.GetLastRect();
            accumulated.width += rect.width;
            accumulated.height += rect.height;
        }
    }
}

