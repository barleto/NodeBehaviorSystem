using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(CutScene))]
public class CutSceneEditor : Editor {

	CutScene cutScene;
	private int index1,index2;
	private CutSceneNode nodeToChange = null;
	private int directionToChange = 0;
	private List<string> listOfSwitches = new List<string>();
	private string csToLoad;

	private int typeIndex = 0;

	public override void OnInspectorGUI()
    {
        cutScene = (CutScene)target;

        CreateNodeListAssetIfnecessary();

        DrawProperties();

        if (cutScene.nodeListAsset == null)
        {
            DrawCreateNodeListAssetButton();
            return;
        }

        DrawNodesList();

        DrawUIForAddingNodes();

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }

    }

    private void DrawCreateNodeListAssetButton()
    {
        if(GUILayout.Button("Create new Node List Asset"))
        {
            CreateCutSceneAsset();
        }
    }

    void CreateCutSceneAsset()
    {
        string nameTag = "behaviourNodeList";
        int fileSufixNumber = 0;
        var config = GetCutSceneSystemConfiguration();
        if (config == null)
        {
            Debug.LogError("Could not find an editor config for BehviorNodeSystem.");
            return;
        }
        int numberedSufix = 0;
        string finalFileName = config.GetPathToSaveNodeLists() + "/" + nameTag + numberedSufix + ".asset";
        var lala = (Application.dataPath + "/" + finalFileName);
        while (File.Exists(Application.dataPath + "/" + finalFileName)) 
        {
            numberedSufix++;
            finalFileName = config.GetPathToSaveNodeLists() + "/" + nameTag + numberedSufix + ".asset";
        }
        var newAsset = cutScene.nodeListAsset = ScriptableObject.CreateInstance<CutSceneNodeList>();
        try
        {
            AssetDatabase.CreateAsset(cutScene.nodeListAsset, "Assets/" + finalFileName);
        } catch (Exception e)
        {
            Debug.LogError("Could not create NodesList asset at path "+ finalFileName);
            return;
        }
        cutScene.nodeListAsset = newAsset;
    }

    private String GenerateAssetName(string nametag)
    {
        throw new NotImplementedException();
    }

    private void DrawProperties()
    {
        //CutSceneSystem
        cutScene.cutSceneSystem = GameObject.Find("CutSceneSystem").GetComponent<CutSceneSystem>();
        cutScene.cutSceneSystem = (CutSceneSystem)EditorGUILayout.ObjectField("Cut Scene System", cutScene.cutSceneSystem, typeof(CutSceneSystem), true);
        cutScene.nodeListAsset = (CutSceneNodeList)EditorGUILayout.ObjectField("Cut Scene Asset:", cutScene.nodeListAsset, typeof(CutSceneNodeList), true);
        if (cutScene.nodeListAsset != null)
        {
            cutScene.nodeListAsset = cutScene.nodeListAsset;
        }
    }


    private void CreateNodeListAssetIfnecessary()
    {
        /*if (cutScene.nodeListAsset == null)
        {
            cutScene.nodeListAsset = new List<CutSceneNode>();
            cutScene.nodeListAsset = ScriptableObject.CreateInstance<CutSceneNodeList>();
            cutScene.nodeListAsset = cutScene.nodeListAsset;
            createCutSceneAsset();

        }*/
    }

    private void DrawNodesList()
    {
        //show list

        for( int i = 0; i < cutScene.nodeListAsset.list.Count() ; i++)
        {
            var node = cutScene.nodeListAsset.list[i];
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("- Delete", GUILayout.Width(100)))
            {
                cutScene.nodeListAsset.list.Remove(node);
                break;
            }
            if (GUILayout.Button("[MOVE UP]", GUILayout.Width(100)))
            {
                if (i == 0)
                {
                    break;
                }
                cutScene.nodeListAsset.list.RemoveAt(i);
                cutScene.nodeListAsset.list.Insert(i - 1, node);
                break;
            }
            if (GUILayout.Button("[MOVE DOWN]", GUILayout.Width(100)))
            {
                if (i == cutScene.nodeListAsset.list.Count()-1)
                {
                    break;
                }
                cutScene.nodeListAsset.list.RemoveAt(i);
                cutScene.nodeListAsset.list.Insert(i + 1, node);
                break;
            }
            GUILayout.EndHorizontal();

            node.createUIDescription(cutScene, serializedObject);

            GUILayout.EndVertical();
        }
    }

    private void DrawUIForAddingNodes()
    {
        //draw buttons for adding and subtracting the cutscene sequence:
        GUILayout.BeginHorizontal();
        var sceneNodeTypesList = Assembly
                           .GetAssembly(typeof(CutSceneNode))
                           .GetTypes()
                           .Where(t => t.IsSubclassOf(typeof(CutSceneNode))).ToArray();
        var sceneNodeTypesnames = sceneNodeTypesList.Select((t) => t.Name).ToArray();

        typeIndex = EditorGUILayout.Popup("Type of node: ", typeIndex, sceneNodeTypesnames);

        if (GUILayout.Button("Add Node", GUILayout.Width(100)))
        {
            var newNode = (CutSceneNode)ScriptableObject.CreateInstance(sceneNodeTypesList[typeIndex]);
            newNode.cutScene = cutScene;
            cutScene.nodeListAsset.list.Add(newNode);
        }
        GUILayout.EndHorizontal();
    }

    [MenuItem("Cut Scene System/Add CutScene Component to gameObject")]
	static void addCutsceneComponentToGameObject(){
		GameObject obj = (GameObject)Selection.activeGameObject;
		if(obj!=null){
			Undo.AddComponent(obj,typeof(CutScene));
		}
	}

    public BehaviorNodeSystemConfiguration GetCutSceneSystemConfiguration()
    {
        var foundList = AssetDatabase.FindAssets("t:BehaviorNodeSystemConfiguration");
        if (foundList.Count() > 0)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(foundList[0]);
            return AssetDatabase.LoadAssetAtPath<BehaviorNodeSystemConfiguration>(assetPath);
        }
        return null;
    }

	// Validate the menu item defined by the function above.
	// The menu item will be disabled if this function returns false.
	[MenuItem ("Cut Scene System/Add CutScene Component to gameObject", true)]
	static bool ValidateaddCutsceneComponentToGameObject () {
		// Return false if no transform is selected.
		return (Selection.activeGameObject != null);
	}

	[MenuItem("Cut Scene System/Add Cut Scene System To Scene")]
	static void CreateCutSceneSytem (MenuCommand command) {
		if(GameObject.Find("CutSceneSystem") == null){
			GameObject sys= Instantiate(Resources.Load("CutSceneSystem", typeof(GameObject))) as GameObject;
			sys.name = "CutSceneSystem";
			Undo.RegisterCreatedObjectUndo(sys, "Create " + sys.name);
        }
	}

	[MenuItem("Cut Scene System/Add Cut Scene System To Scene",true)]
	static bool ValidateCreateCutSceneSytem (MenuCommand command) {
		if(GameObject.Find("CutSceneSystem") == null){
			return true;
		}
		return false;
	}

	void createNodeAsset(CutSceneNode node){
		string url = AssetDatabase.GetAssetPath (cutScene.nodeListAsset);
		int index = AssetDatabase.GetAssetPath (cutScene.nodeListAsset).LastIndexOf ("/");
		url = url.Substring (0,index);
		int fileName = 0;
		while(AssetDatabase.LoadAssetAtPath<CutSceneNode>(url+"/nodes/node"+fileName+".asset") != null){
			fileName++;
		}
		AssetDatabase.CreateAsset (node,url+"/nodes/node"+fileName+".asset");
	}
}
#endif