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

    private bool _isConnecting;
    private VisualNodeBase _parentlessNode;
    Vector2 scrollPos;
    Rect fullEditorSize = Rect.zero;
    private bool _isDragging;
    Vector2 lastMousePos;
    float zoomScale = 1f;

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
            _currentRoot.nodes.RemoveAll((o) => o == null);
        }
        zoomScale = 1f;
        scrollPos = Vector2.zero;
        Repaint();
    }

    void AddNodeToRoot(object o)
    {
        var nodeType = (Type)o;
        var node = new VisualNodeAssetHelper().CreateNodeAsset(_currentRoot, nodeType);
        node.windowPosition = new Rect(0, 0, 150, 0);
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

    private void OnFocus()
    {
        if (_currentRoot != null)
        {
            _currentRoot.nodes.RemoveAll((o) => o == null);
            foreach (var node in _currentRoot.nodes)
            {
               node.windowPosition.height = 0;
            }
        }
    }

    private void OnGUI()
    {
        ZoomWindow(1);

        GUI.Box(new Rect(0, 0, position.width, 20), "");
        _currentRoot = (VisualNodeRoot)EditorGUILayout.ObjectField(_currentRoot, typeof(VisualNodeRoot));
        
        if (_currentRoot == null)
        {
            GUILayout.Label("There is no VisualNodeRoot selected.");
            return;
        }

        if (_currentRoot.root == null)
        {
            if (GUI.Button(new Rect(0, 20, 100, 16), "Add root node"))
            {
                new VisualNodeEditorContextMenu(AddNodeToRoot).ShowMenu();
            }
            return;
        }

        if (Event.current.isScrollWheel)
        {
            zoomScale += Event.current.delta.y / 100;
            if (zoomScale <= .1f)
            {
                zoomScale = .1f;
            }
        }

        //Begin scroll view
        fullEditorSize.width += 50;
        fullEditorSize.height += 50;
        scrollPos = GUI.BeginScrollView(new Rect(0, 20, position.width, position.height - 20), scrollPos, fullEditorSize);
        fullEditorSize.xMax = 0;
        fullEditorSize.yMax = 0;


        DrawNodesConnections();

        DrawNodeWindows();

        GUI.EndScrollView();

        ConnectingToParentEventCheck();

        HandleContextClick();

        HandleScrollDrag();
    }

    private void DrawNodeWindows()
    {
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
    }

    private void DrawNodesConnections()
    {
        //Draw node connections
        for (int i = 0; i < _currentRoot.nodes.Count(); i++)
        {
            var node = _currentRoot.nodes[i];
            if (node == null)
            {
                continue;
            };
            for (int j = 0; j < node.children.Count(); j++)
            {
                var child = node.children[j];
                if (child == null){
                    continue;
                }
                var heightSector = node.windowPosition.height / node.ChildMax();
                var curveRect = new Rect(
                    node.windowPosition.xMax,
                    node.windowPosition.y + heightSector * (j + 1) - heightSector / 2,
                    0,
                    0);
                var lineColor = j < node.LineColor().Length ? node.LineColor()[j] : Color.gray;
                DrawNodeCurve(curveRect, child.windowPosition, lineColor);
            }
        }
    }

    private void HandleScrollDrag()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            lastMousePos = Event.current.mousePosition;
            _isDragging = true;
            Event.current.Use();
        }
        else if ((Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseLeaveWindow))
        {
            _isDragging = false;
        }

        if (_isDragging)
        {
            var movement = Event.current.mousePosition - lastMousePos;
            movement = -movement;
            lastMousePos = Event.current.mousePosition;
            scrollPos += movement;
            Repaint();
            scrollPos.x = Mathf.Clamp(scrollPos.x, 0, scrollPos.x);
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0, scrollPos.y);
        }
    }

    private void HandleContextClick()
    {
        if (Event.current.button == 1 && mouseOverWindow == this)
        {
            Event.current.Use();
            var mousePos = Event.current.mousePosition;
            new VisualNodeEditorContextMenu((nodeType) =>
            {
                var newNode = new VisualNodeAssetHelper().CreateNodeAsset(_currentRoot, nodeType);
                mousePos += scrollPos;
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
            DrawNodeCurve(new Rect(Event.current.mousePosition, Vector2.zero), FixRectWithScroll(_parentlessNode.windowPosition));
            Repaint();
            foreach (var node in _currentRoot.nodes)
            {
                Rect nodeWindowPosition = FixRectWithScroll(node.windowPosition);
                nodeWindowPosition.y += 20;
                if (node != _parentlessNode && nodeWindowPosition.Contains(Event.current.mousePosition))
                {
                    _isConnecting = false;
                    if (node.children.Count() < node.ChildMax())
                    {
                        _parentlessNode.parent = node;
                    }
                    _parentlessNode.parent = node;
                    var heightSector = node.windowPosition.height / node.ChildMax();
                    var childIndex = Mathf.CeilToInt((Event.current.mousePosition.y - nodeWindowPosition.y) / heightSector) - 1;
                    if (node.children.Count() > childIndex && node.children[childIndex] != null)
                    {
                        node.children[childIndex].parent = null;
                    }
                    node.children.RemoveAt(childIndex);
                    node.children.Insert(childIndex, _parentlessNode);
                    SetNodeForSaving(node);
                    SetNodeForSaving(_parentlessNode);
                    break;
                }
            }
        }
    }

    Rect FixRectWithScroll(Rect rect)
    {
        return AddVectorToRect(rect, -scrollPos);
    }

    Rect AddVectorToRect(Rect rect, Vector2 vec2)
    {
        rect.position += vec2;
        return rect;
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
        node.windowPosition.y = Mathf.Clamp(node.windowPosition.y, 0F, node.windowPosition.y);
        node.VerifyChildrenVectorSize();
        for (int i = 0; i < node.ChildMax(); i++)
        {
            if (node.children[i] == null) {
                DrawPlusButtonsOnNode(node, i);
            }
        }
    }

    [MenuItem("Assets/Open in Visual Node Editor")]
    public static void OpenInVisualNodeEditor()
    {
        ShowWindow();
        _currentRoot = Selection.activeObject as VisualNodeRoot;
    }

    [MenuItem("Assets/Open in Visual Node Editor", true)]
    public static bool ValidateOpenInVisualNodeEditor()
    {
        return Selection.activeObject is VisualNodeRoot;
    }

    private void DrawDeleteButton(VisualNodeBase node)
    {
        if (_isConnecting)
        {
            return;
        }
        if (GUI.Button(new Rect(0, 0, 25,16),"×"))
        {
            new VisualNodeAssetHelper().DeleteNodes(node, _currentRoot);
            AssetDatabase.SaveAssets();
        }

        if (GUI.Button(new Rect(node.windowPosition.width - 50, 0, 50, 16), "×Rec"))
        {
            if (EditorUtility.DisplayDialog("Recursive delete","This will delete nodes recurssively. Are you sure you want to continue?", "Yes", "Cancel")) {
                new VisualNodeAssetHelper().DeleteNodesRecursive(node, _currentRoot);
                AssetDatabase.SaveAssets();
            }
        }
    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.gray, null, 2);
    }

    void DrawNodeCurve(Rect start, Rect end, Color color)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 2);
    }

    private void DrawPlusButtonsOnNode(VisualNodeBase node, int i)
    {
        var childIndex = i;
        var butLabel = i < node.ChildrenLabelsInEditor().Length ? node.ChildrenLabelsInEditor()[i] : "+";
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

    private void ZoomWindow(float zoomScale)
    {
        //Scale gui matrix
        Vector2 vanishingPoint = new Vector2(0, 20/zoomScale);
        Matrix4x4 Translation = Matrix4x4.TRS(vanishingPoint, Quaternion.identity, Vector3.one);
        Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
        GUI.matrix = Translation * Scale * Translation.inverse;

        //GUIUtility.ScaleAroundPivot(Vector2.one * zoomScale, Vector2.zero);
    }
}
