using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ZoomTestWindow : EditorWindow
{
    [MenuItem("Window/Zoom Test")]
    private static void Init()
    {
        ZoomTestWindow window = (ZoomTestWindow)GetWindow(typeof(ZoomTestWindow) ,false, "Zoom Test");
        window.minSize = new Vector2(600.0f, 300.0f);
        window.wantsMouseMove = true;
        window.Show();
    }

    private EditorZoomArea editorZoomArea;

    private void DrawZoomArea()
    {
        // Within the zoom area all coordinates are relative to the top left corner of the zoom area
        // with the width and height being scaled versions of the original/unzoomed area's width and height.
        editorZoomArea.Begin();

        // You can also use GUILayout inside the zoomed area.
        GUI.Box(new Rect(100.0f, 100.0f, 100.0f, 25.0f), "Zoomed Box");
        GUILayout.Button("Zoomed Button 1");
        GUILayout.Button("Zoomed Button 2");

        editorZoomArea.End();
    }
    
    private void OnSelectionChange()
    {
        editorZoomArea.ResetZoom();
        Repaint();
    }

    public void OnGUI()
    {
        if (editorZoomArea == null)
        {
            editorZoomArea = new EditorZoomArea(new Rect(0.0f, 0f, position.width, position.height));
        }

        // The zoom area clipping is sometimes not fully confined to the passed in rectangle. At certain
        // zoom levels you will get a line of pixels rendered outside of the passed in area because of
        // floating point imprecision in the scaling. Therefore, it is recommended to draw the zoom
        // area first and then draw everything else so that there is no undesired overlap.
        DrawZoomArea();
    }
}

public class EditorZoomArea
{
    private const float kEditorWindowTabHeight = 21.0f;
    private Matrix4x4 _prevGuiMatrix;
    private Vector2 _zoomCoordsOrigin = Vector2.zero;
    private float _zoom = 1f;
    private Rect _zoomArea = new Rect(0,0,200,200);
    private const float kZoomMin = 0.1f;
    private const float kZoomMax = 10.0f;
    private const float wheelSensibility = 100.0f;

    public EditorZoomArea(Rect position)
    {
        _zoomArea = position;
    }

    public Rect Begin()
    {
        HandleEvents();
        GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

        Rect clippedArea = _zoomArea.ScaleSizeBy(1.0f / _zoom, _zoomArea.TopLeft());
        clippedArea.y += kEditorWindowTabHeight;
        GUI.BeginGroup(clippedArea);

        _prevGuiMatrix = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(_zoom, _zoom, 1.0f));
        GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

        GUILayout.BeginArea(new Rect(_zoomCoordsOrigin.x, _zoomCoordsOrigin.y, _zoomArea.width, _zoomArea.height));

        return clippedArea;
    }

    private void HandleEvents()
    {

        // Allow adjusting the zoom with the mouse wheel as well. In this case, use the mouse coordinates
        // as the zoom center instead of the top left corner of the zoom area. This is achieved by
        // maintaining an origin that is used as offset when drawing any GUI elements in the zoom area.
        if (Event.current.type == EventType.ScrollWheel)
        {
            Vector2 screenCoordsMousePos = Event.current.mousePosition;
            Vector2 delta = Event.current.delta;
            Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
            float zoomDelta = -delta.y / (wheelSensibility * 1f/_zoom);
            float oldZoom = _zoom;
            _zoom += zoomDelta;
            _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
            var lala = (_zoom / oldZoom) * (zoomCoordsMousePos) - zoomCoordsMousePos;
            _zoomCoordsOrigin -= lala;
            _zoomCoordsOrigin.x = Mathf.Min(0f, _zoomCoordsOrigin.x);
            _zoomCoordsOrigin.y = Mathf.Min(0f, _zoomCoordsOrigin.y);
            Event.current.Use();
        }

        // Allow moving the zoom area's origin by dragging with the middle mouse button or dragging
        // with the left mouse button with Alt pressed.
        if (Event.current.type == EventType.MouseDrag &&
            (Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
            Event.current.button == 2)
        {
            Vector2 delta = Event.current.delta;
            delta /= _zoom;
            _zoomCoordsOrigin += delta;
            _zoomCoordsOrigin.x = Mathf.Min(0f, _zoomCoordsOrigin.x);
            _zoomCoordsOrigin.y = Mathf.Min(0f, _zoomCoordsOrigin.y);
            Event.current.Use();
        }
    }

    private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
    {
        return (screenCoords - _zoomArea.TopLeft()) / _zoom;
    }

    public void ResetZoom()
    {
        _zoom = 1;
        _zoomCoordsOrigin = Vector2.zero;
    }

    public void End()
    {
        GUI.matrix = _prevGuiMatrix;
        GUI.EndGroup();
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
    }
}

public static class RectExtensions
{
    public static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }
    public static Rect ScaleSizeBy(this Rect rect, float scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }
    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.xMax *= scale;
        result.yMin *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale.x;
        result.xMax *= scale.x;
        result.yMin *= scale.y;
        result.yMax *= scale.y;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
}
 