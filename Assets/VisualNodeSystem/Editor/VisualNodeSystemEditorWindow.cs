using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VisualNodeSystemEditorWindow : EditorWindow {

    VisualNodeRoot _currentRoot;
    Rect Windows;
    private int _nodeIndex;
    Dictionary<int, VisualNodeBase> _nodeWindowDictionary = new Dictionary<int, VisualNodeBase>();

    private bool _isConnecting;
    private VisualNodeBase _parentlessNode;
    Vector2 scrollPos;
    Rect fullEditorSize = Rect.zero;

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

    void AddNodeToRoot(object o)
    {
        var nodeType = (Type)o;
        var node = new VisualNodeAssetHelper().CreateNodeAsset(_currentRoot, nodeType);
        node.windowPosition = new Rect(0, 19, 150, 0);
        node.parent = null;
        _currentRoot.root = node;
        _currentRoot.nodes.Add(node);
        SaveRootAsset();
    }

    private void SaveRootAsset()
    {
        EditorUtility.SetDirty(_currentRoot);
        AssetDatabase.SaveAssets();
    }

    private void SetNodeForSaving(VisualNodeBase node)
    {
        EditorUtility.SetDirty(node);
    }

    private void OnGUI()
    {

        if (_currentRoot == null)
        {
            GUILayout.Label("There is no VisualNodeRoot selected.");
            return;
        }

        GUI.Box(new Rect(0, 0, position.width, 20), _currentRoot.name);
        if (_currentRoot.root == null)
        {
            if (GUI.Button(new Rect(0, 20, 100, 16), "Add root node"))
            {
                new VisualNodeEditorContextMenu(AddNodeToRoot).ShowMenu();
            }
            return;
        }

        //Begin scroll view
        scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, fullEditorSize);
        fullEditorSize.xMax = 0;
        fullEditorSize.yMax = 0;

        //Draw node connections
        for (int i = 0; i < _currentRoot.nodes.Count(); i++)
        {
            var node = _currentRoot.nodes[i];
            if (node == null){
                continue;
            };
            if (node.parent != null)
            {
                var parent = node.parent;
                var nodeAsChildPos = parent.children.IndexOf(node);
                var heightSector = parent.windowPosition.height / parent.ChildMax();
                var curveRect = new Rect(
                    parent.windowPosition.xMax,
                    parent.windowPosition.y + heightSector * (nodeAsChildPos + 1) - heightSector / 2,
                    0,
                    0);
                DrawNodeCurve(curveRect, node.windowPosition);
            }
        }
        //draw node windows
        _nodeIndex = 0;
        BeginWindows();
        for (int i = 0; i < _currentRoot.nodes.Count(); i++)
        {
            var node = _currentRoot.nodes[i];
            DrawNodeDecorations(node);
            if (node.parent == null && _currentRoot.root != node && !_isConnecting)
            {
                if (GUI.Button(new Rect(node.windowPosition.x - 18, node.windowPosition.center.y - 10, 20, 20), "P"))
                {
                    _isConnecting = true;
                    _parentlessNode = node;
                }
            }
            if (node.parent != null)
            {
                if (GUI.Button(new Rect(node.windowPosition.x - 18, node.windowPosition.center.y - 10, 20, 16), "×"))
                {
                    var index = node.parent.children.IndexOf(node);
                    node.parent.children.RemoveAt(index);
                    node.parent.children.Insert(index, null);
                    node.parent = null;
                }
            }

            fullEditorSize.xMax = Mathf.Max(fullEditorSize.xMax, node.windowPosition.xMax);
            fullEditorSize.yMax = Mathf.Max(fullEditorSize.yMax, node.windowPosition.yMax);
        }
        EndWindows();

        GUI.EndScrollView();

        ConnectingToParentEventCheck();

        if (Event.current.type == EventType.ContextClick)
        {
            var mousePos = Event.current.mousePosition;
            new VisualNodeEditorContextMenu((nodeType) =>
            {
                var newNode = new VisualNodeAssetHelper().CreateNodeAsset(_currentRoot, nodeType);
                newNode.windowPosition = new Rect(mousePos.x, mousePos.y, 150, 0);
                newNode.parent = null;
                _currentRoot.nodes.Add(newNode);
                SaveRootAsset();
            }).ShowMenu();
        }
    }

    private void ConnectingToParentEventCheck()
    {
        if (_isConnecting)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                _isConnecting = false;
            }
            DrawNodeCurve(new Rect(Event.current.mousePosition, Vector2.zero), _parentlessNode.windowPosition);
            Repaint();
            foreach (var node in _currentRoot.nodes)
            {
                if (node != _parentlessNode && node.windowPosition.Overlaps(new Rect(Event.current.mousePosition, Vector2.zero)))
                {
                    _isConnecting = false;
                    if (node.children.Count() < node.ChildMax())
                    {
                        _parentlessNode.parent = node;
                    }
                    _parentlessNode.parent = node;
                    var heightSector = node.windowPosition.height / node.ChildMax();
                    var childIndex = Mathf.CeilToInt((Event.current.mousePosition.y - node.windowPosition.y) / heightSector) - 1;
                    if (node.children.Count() > childIndex && node.children[childIndex] != null)
                    {
                        node.children[childIndex].parent = null;
                    }
                    node.children.RemoveAt(childIndex);
                    node.children.Insert(childIndex, _parentlessNode);
                    SetNodeForSaving(node);
                    SetNodeForSaving(_parentlessNode);
                }
            }
        }
    }

    void DrawNodeDecorations(VisualNodeBase node)
    {
        _nodeIndex++;
        _nodeWindowDictionary[_nodeIndex] = node;
        var newPos = GUILayout.Window(_nodeIndex, node.windowPosition, DrawNodeWindow, node.GetType().Name);
        if (newPos != node.windowPosition)
        {
            EditorUtility.SetDirty(node);
        }
        node.windowPosition = newPos;
        node.windowPosition.x = Mathf.Clamp(node.windowPosition.x, 0f, node.windowPosition.x);
        node.windowPosition.y = Mathf.Clamp(node.windowPosition.y, 19f, node.windowPosition.y);
        node.VerifyChildrenVectorSize();
        for (int i = 0; i < node.ChildMax(); i++)
        {
            if (node.children[i] == null) {
                DrawPlusButtonsOnNode(node, i);
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

    private void DrawPlusButtonsOnNode(VisualNodeBase node, int i)
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
                SaveRootAsset();
                SetNodeForSaving(node);
                SetNodeForSaving(newNode);
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
