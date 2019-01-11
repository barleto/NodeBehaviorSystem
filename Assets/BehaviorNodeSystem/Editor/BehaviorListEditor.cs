using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

namespace BehaviorNodePlugin
{

    [CustomEditor(typeof(BehaviorListHolder))]
    public class BehaviorListEditor : Editor
    {

        BehaviorListHolder behaviorList;
        private List<string> listOfSwitches = new List<string>();

        private int typeIndex = 0;

        private static bool _hasRegisteredUndo = false;

        public override void OnInspectorGUI()
        {
            behaviorList = (BehaviorListHolder)target;

            TryRegisterUndoCallback();

            DrawDefaultInspector();

            if (behaviorList.nodeListAsset == null)
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
            if (GUILayout.Button("Create new Node List Asset"))
            {
                CreateBehaviorListAsset();
            }
        }

        void CreateBehaviorListAsset()
        {
            var config = GetBehaviorNodeSystemConfiguration();
            if (config == null)
            {
                Debug.LogError("Could not find an editor config for BehviorNodeSystem.");
                return;
            }
            var assetCreator = new BehaviorNodeAssetCreator(config.GetPathToSaveNodeLists());
            behaviorList.nodeListAsset = assetCreator.CreateNodeListAsset();
        }

        private String GenerateAssetName(string nametag)
        {
            throw new NotImplementedException();
        }


        private void DrawNodesList()
        {
            //show list

            for (int i = 0; i < behaviorList.nodeListAsset.list.Count(); i++)
            {
                var node = behaviorList.nodeListAsset.list[i];
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    behaviorList.nodeListAsset.list.Remove(node);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
                    break;
                }
                if (GUILayout.Button("▲", GUILayout.Width(30)))
                {
                    if (i == 0)
                    {
                        break;
                    }
                    Undo.RecordObject(behaviorList.nodeListAsset, "Node moved up");
                    behaviorList.nodeListAsset.list.RemoveAt(i);
                    behaviorList.nodeListAsset.list.Insert(i - 1, node);
                    break;
                }
                if (GUILayout.Button("▼", GUILayout.Width(30)))
                {
                    if (i == behaviorList.nodeListAsset.list.Count() - 1)
                    {
                        break;
                    }
                    Undo.RecordObject(behaviorList.nodeListAsset, "Node moved down");
                    behaviorList.nodeListAsset.list.RemoveAt(i);
                    behaviorList.nodeListAsset.list.Insert(i + 1, node);
                    break;
                }
                if (GUILayout.Button(new GUIContent("T", "Send node to bottom"), GUILayout.Width(30)))
                {
                    Undo.RecordObject(behaviorList.nodeListAsset, "Node moved top");
                    behaviorList.nodeListAsset.list.RemoveAt(i);
                    behaviorList.nodeListAsset.list.Insert(0, node);
                    break;
                }
                if (GUILayout.Button(new GUIContent("B", "Send node to bottom"), GUILayout.Width(30)))
                {
                    Undo.RecordObject(behaviorList.nodeListAsset, "Node moved bottom");
                    behaviorList.nodeListAsset.list.RemoveAt(i);
                    behaviorList.nodeListAsset.list.Insert(behaviorList.nodeListAsset.list.Count(), node);
                    break;
                }
                GUILayout.EndHorizontal();

                Undo.RecordObject(node, "Node properties changed.");
                EditorGUI.BeginChangeCheck();
                if (node.HasCustomInspector())
                {
                    node.createUIDescription(behaviorList, serializedObject);
                }
                else
                {
                    Editor.CreateEditor(node).OnInspectorGUI();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(node);
                }
                GUILayout.EndVertical();
            }
        }

        private void DrawUIForAddingNodes()
        {
            //draw buttons for adding and subtracting the behavior list sequence:
            GUILayout.BeginHorizontal();
            var sceneNodeTypesList = Assembly
                               .GetAssembly(typeof(BehaviorNode))
                               .GetTypes()
                               .Where(t => t.IsSubclassOf(typeof(BehaviorNode))).ToArray();
            var sceneNodeTypesnames = sceneNodeTypesList.Select((t) => t.Name).ToArray();

            typeIndex = EditorGUILayout.Popup("Type of node: ", typeIndex, sceneNodeTypesnames);

            if (GUILayout.Button("Add Node", GUILayout.Width(100)))
            {
                var config = GetBehaviorNodeSystemConfiguration();
                if (config == null)
                {
                    Debug.LogError("Could not find an editor config for BehviorNodeSystem.");
                    return;
                }
                var assetCreator = new BehaviorNodeAssetCreator(config.GetPathToSaveNodeLists());
                var newNode = assetCreator.CreateNodeAsset(behaviorList.nodeListAsset, sceneNodeTypesList[typeIndex]);
                behaviorList.nodeListAsset.list.Add(newNode);
            }
            GUILayout.EndHorizontal();
        }

        public static BehaviorNodeSystemConfiguration GetBehaviorNodeSystemConfiguration()
        {
            var config = Resources.Load<BehaviorNodeSystemConfiguration>("BehaviorNodeSystemConfiguration");
            if (config != null)
            {
                return config;
            }
            return null;
        }

        const string pluginName = "Plugins/BehaviourNodes";

        [MenuItem(pluginName + "/Create new BehaviorNodeList Asset")]
        static void CreateNewBehaviorListAsset()
        {
            var config = GetBehaviorNodeSystemConfiguration();
            if (config == null)
            {
                Debug.LogError("Could not find an editor config for BehviorNodeSystem.");
                return;
            }
            var assetCreator = new BehaviorNodeAssetCreator(config.GetPathToSaveNodeLists());
            EditorGUIUtility.PingObject(assetCreator.CreateNodeListAsset());
        }

        [MenuItem(pluginName + "/Add NodeBehavior System To Scene")]
        static void CreateBehaviorNodeSytem(MenuCommand command)
        {
            var config = GetBehaviorNodeSystemConfiguration();
            if (config == null)
            {
                Debug.LogError("Could not find an editor config for BehviorNodeSystem.");
                return;
            }
            var go = Instantiate(config.GetSystemPrefab());
            go.name = config.GetSystemPrefab().name;
            Undo.RegisterCreatedObjectUndo(go, "Create BehaviorSystem");
        }

        [MenuItem(pluginName + "/Add NodeBehavior System To Scene", true)]
        static bool ValidateCreateBehaviorNodeSytem(MenuCommand command)
        {
            var config = GetBehaviorNodeSystemConfiguration();
            if (config == null)
            {
                return false;
            }
            if (GameObject.Find(config.GetSystemPrefab().name) == null)
            {
                return true;
            }
            return false;
        }

        void createNodeAsset(BehaviorNode node)
        {
            string url = AssetDatabase.GetAssetPath(behaviorList.nodeListAsset);
            int index = AssetDatabase.GetAssetPath(behaviorList.nodeListAsset).LastIndexOf("/");
            url = url.Substring(0, index);
            int fileName = 0;
            while (AssetDatabase.LoadAssetAtPath<BehaviorNode>(url + "/nodes/node" + fileName + ".asset") != null)
            {
                fileName++;
            }
            AssetDatabase.CreateAsset(node, url + "/nodes/node" + fileName + ".asset");
        }

        private void TryRegisterUndoCallback()
        {
            if (!_hasRegisteredUndo)
            {
                Undo.undoRedoPerformed += OnUndo;
                _hasRegisteredUndo = true;
            }
        }

        private void OnUndo()
        {
            Repaint();
        }
    }
}
#endif