using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class VisualNodeEditorContextMenu {

    Action<Type> _callback;
    GenericMenu _genericMenu;

	public VisualNodeEditorContextMenu(Action<Type> callback)
    {
        _genericMenu = new GenericMenu();
        _callback = callback;
        FillGenericMenu();
    }

    private void FillGenericMenu()
    {
        _genericMenu.AddDisabledItem(new GUIContent("Add Node :"));
        _genericMenu.AddSeparator("");
        var nodesTypesList = Assembly
                               .GetAssembly(typeof(VisualNodeBase))
                               .GetTypes()
                               .Where(t => t.IsSubclassOf(typeof(VisualNodeBase))).ToArray();
        foreach (var type in nodesTypesList)
        {
            AddMenuItem(type);
        }
    }

    public void ShowMenu()
    {
        _genericMenu.ShowAsContext();
    }

    void AddMenuItem(Type type)
    {
        _genericMenu.AddItem(new GUIContent(type.Name), false, ContextMenuCallback, type);
    }

    private void ContextMenuCallback(object o)
    {
        var type = (Type)o;
        _callback(type);
    }
}
