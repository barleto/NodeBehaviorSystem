using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VisualNodeAssetHelper {

    public T CreateNodeAsset<T>(VisualNodeRoot root) where T : ScriptableObject
    {
        var nodeNameTag = typeof(T).Name;
        var path = AssetDatabase.GetAssetPath(root);
        var directoryPath = Path.GetDirectoryName(path);
        var absolutePath = Path.GetDirectoryName(Application.dataPath) + "/";
        int numberedSufix = -1;
        string nodeFinalPath;
        do
        {
            numberedSufix++;
            nodeFinalPath = directoryPath + "/" + nodeNameTag + numberedSufix + ".asset";
        } while (File.Exists(absolutePath + nodeFinalPath));

        var newNode = ScriptableObject.CreateInstance<T>();
        try
        {
            AssetDatabase.CreateAsset(newNode, nodeFinalPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not create Node asset at path " + nodeFinalPath);
            return null;
        }
        return newNode;
    }

    public VisualNodeBase CreateNodeAsset(VisualNodeRoot root, Type type)
    {
        var nodeNameTag = type.Name;
        var path = AssetDatabase.GetAssetPath(root);
        var directoryPath = Path.GetDirectoryName(path);
        var absolutePath = Path.GetDirectoryName(Application.dataPath) + "/";
        int numberedSufix = -1;
        string nodeFinalPath;
        do
        {
            numberedSufix++;
            nodeFinalPath = directoryPath + "/" + nodeNameTag + numberedSufix + ".asset";
        } while (File.Exists(absolutePath + nodeFinalPath));

        var newNode = ScriptableObject.CreateInstance(type);
        try
        {
            AssetDatabase.CreateAsset(newNode, nodeFinalPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not create Node asset at path " + nodeFinalPath);
            return null;
        }
        return (VisualNodeBase)newNode;
    }

    public void DeleteNodesRecursive(VisualNodeBase node, VisualNodeRoot root)
    {
        //enter on children
        var childCount = node.children.Count();
        for (int i = childCount-1; i >= 0; i--)
        {
            if (node.children[i] != null) {
                node.children[i].parent = null;
            }
        }
        //remove from parent
        if (node.parent != null)
        {
            node.parent.children.RemoveAll((o)=>node == o);
        }

        root.nodes.Remove(node);

        //delete asset
        var path = AssetDatabase.GetAssetPath(node);
        AssetDatabase.DeleteAsset(path);
    }
}
