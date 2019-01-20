using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VisualNodeSystemEditorWindow : EditorWindow {

    static VisualNodeRoot _currentRoot;
    Rect Windows;
    private int _nodeIndex;
    Dictionary<int, VisualNodeBase> _nodeWindowDictionary = new Dictionary<int, VisualNodeBase>();

    [MenuItem("Window/VisualNodeSystem")]
    static void ShowWindow()
    {
        var windoww = (VisualNodeSystemEditorWindow)EditorWindow.GetWindow(typeof(VisualNodeSystemEditorWindow));
        windoww.Show();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeObject is VisualNodeRoot)
        {
            _currentRoot = Selection.activeObject as VisualNodeRoot;
        }
        Repaint();
    }

    void AddNode(object o)
    {

    }

    void AddNodeToRoot(object o)
    {
        var nodeType = (Type)o;
        var node = new VisualNodeAssetHelper().CreateNodeAsset(_currentRoot,nodeType);
        node.windowPosition = new Rect(0,32,150,0);
        node.parent = null;
        _currentRoot.nodes.Clear();
        _currentRoot.root = node;
        _currentRoot.nodes.Add(node);
    }

    private void OnGUI()
    {
        if (_currentRoot == null)
        {
            GUILayout.Label("There is no VisualNodeRoot selected.");
            return;
        }
        
        GUI.Box(new Rect(0,0,100,32),"ROOT");
        if (_currentRoot.root == null) {
            if (GUI.Button(new Rect(0, 16, 100, 16), "Add root node"))
            {
                new VisualNodeEditorContextMenu(AddNodeToRoot).ShowMenu();
            }
            return;
        }
        //lala
        GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), Vector2.zero, new Rect(0, 0, 2*position.width, 2*position.height));
        _nodeIndex = 0;
        BeginWindows();
        for (int i = 0; i < _currentRoot.nodes.Count(); i++)
        {
            DrawNodeDecorations(_currentRoot.nodes[i]);
        }
        
        EndWindows();
        GUI.EndScrollView();
    }

    void DrawNodeDecorations(VisualNodeBase node)
    {
        _nodeIndex++;
        _nodeWindowDictionary[_nodeIndex] = node;
        node.windowPosition = GUILayout.Window(_nodeIndex, node.windowPosition, DrawNodeWindow, node.GetType().Name);
        node.windowPosition.x = Mathf.Clamp(node.windowPosition.x, 0f, node.windowPosition.x);
        node.windowPosition.y = Mathf.Clamp(node.windowPosition.y, 32f, node.windowPosition.y);
        node.VerifyChildrenVectorSize();
        for (int i = 0; i < node.ChildMax(); i++)
        {
            if (node.children[i] == null) {
                DrawPlusButtonOnNode(node, i);
            }
            else
            {
                DrawNodeCurve(node.windowPosition, node.children[i].windowPosition);
            }
        }
    }

    private void DrawDeleteButton(VisualNodeBase node)
    {
        if (GUI.Button(new Rect(0, 0, 25,16),"×"))
        {
            new VisualNodeAssetHelper().DeleteNodesRecursive(node, _currentRoot);
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

    private static void DrawPlusButtonOnNode(VisualNodeBase node, int i)
    {
        var childIndex = i;
        var butLabel = node.ChildrenLabels()[i];
        float windowFraction = node.windowPosition.height / node.ChildMax();
        var butRect = new Rect(
            node.windowPosition.xMax, 
            node.windowPosition.y + (i)*(windowFraction) + windowFraction/2 - 10, 
            Mathf.Min(10*butLabel.Length+10, 50), 
            20);
        if (GUI.Button(butRect, butLabel))
        {
            new VisualNodeEditorContextMenu((nodeType) =>
            {
                var newNode = new VisualNodeAssetHelper().CreateNodeAsset(_currentRoot, nodeType);
                newNode.windowPosition = new Rect(node.windowPosition.xMax + 20, node.windowPosition.y, 150, 0);
                newNode.parent = node;
                node.children.RemoveAt(childIndex);
                node.children.Insert(childIndex, newNode);
                _currentRoot.nodes.Add(newNode);
            }).ShowMenu();
        }
    }

    private void DrawNodeWindow(int id)
    {
        VisualNodeBase node = null;
        _nodeWindowDictionary.TryGetValue(id, out node);
        if (node != null) {
            DrawDeleteButton(node);
            EditorGUI.BeginChangeCheck();
            try{
                Editor.CreateEditor(node).OnInspectorGUI();
            }catch(Exception e)
            {
                return;
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(node);
            }
        }
        GUI.DragWindow();
    }
}
