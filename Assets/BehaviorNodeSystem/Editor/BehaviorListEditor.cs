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

        BehaviorListHolder behaviorListHolder;
        private List<string> listOfSwitches = new List<string>();

        private int typeIndex = 0;

        private static bool _hasRegisteredUndo = false;

        public override void OnInspectorGUI()
        {
            behaviorListHolder = (BehaviorListHolder)target;

            TryRegisterUndoCallback();

            DrawDefaultInspector();

            if (behaviorListHolder.nodeListAsset == null)
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
            behaviorListHolder.nodeListAsset = assetCreator.CreateNodeListAsset();
        }

        private String GenerateAssetName(string nametag)
        {
            throw new NotImplementedException();
        }


        private void DrawNodesList()
        {
            //show list
            bool hasListChanged = false;

            for (int i = 0; i < behaviorListHolder.nodeListAsset.list.Count(); i++)
            {
                var node = behaviorListHolder.nodeListAsset.list[i];
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    behaviorListHolder.nodeListAsset.list.Remove(node);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node));
                    hasListChanged = true;
                    break;
                }
                if (GUILayout.Button("▲", GUILayout.Width(30)))
                {
                    if (i == 0)
                    {
                        break;
                    }
                    Undo.RecordObject(behaviorListHolder.nodeListAsset, "Node moved up");
                    behaviorListHolder.nodeListAsset.list.RemoveAt(i);
                    behaviorListHolder.nodeListAsset.list.Insert(i - 1, node);
                    hasListChanged = true;
                    break;
                }
                if (GUILayout.Button("▼", GUILayout.Width(30)))
                {
                    if (i == behaviorListHolder.nodeListAsset.list.Count() - 1)
                    {
                        break;
                    }
                    Undo.RecordObject(behaviorListHolder.nodeListAsset, "Node moved down");
                    behaviorListHolder.nodeListAsset.list.RemoveAt(i);
                    behaviorListHolder.nodeListAsset.list.Insert(i + 1, node);
                    hasListChanged = true;
                    break;
                }
                if (GUILayout.Button(new GUIContent("T", "Send node to bottom"), GUILayout.Width(30)))
                {
                    Undo.RecordObject(behaviorListHolder.nodeListAsset, "Node moved top");
                    behaviorListHolder.nodeListAsset.list.RemoveAt(i);
                    behaviorListHolder.nodeListAsset.list.Insert(0, node);
                    hasListChanged = true;
                    break;
                }
                if (GUILayout.Button(new GUIContent("B", "Send node to bottom"), GUILayout.Width(30)))
                {
                    Undo.RecordObject(behaviorListHolder.nodeListAsset, "Node moved bottom");
                    behaviorListHolder.nodeListAsset.list.RemoveAt(i);
                    behaviorListHolder.nodeListAsset.list.Insert(behaviorListHolder.nodeListAsset.list.Count(), node);
                    hasListChanged = true;
                    break;
                }
                GUILayout.EndHorizontal();

                Undo.RecordObject(node, "Node properties changed.");
                EditorGUI.BeginChangeCheck();
                if (node.HasCustomInspector())
                {
                    node.createUIDescription(behaviorListHolder, serializedObject);
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

            if (hasListChanged)
            {
                EditorUtility.SetDirty(behaviorListHolder.nodeListAsset);
                AssetDatabase.SaveAssets();
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
                var newNode = assetCreator.CreateNodeAsset(behaviorListHolder.nodeListAsset, sceneNodeTypesList[typeIndex]);
                behaviorListHolder.nodeListAsset.list.Add(newNode);
                EditorUtility.SetDirty(behaviorListHolder);
                AssetDatabase.SaveAssets();
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
            string url = AssetDatabase.GetAssetPath(behaviorListHolder.nodeListAsset);
            int index = AssetDatabase.GetAssetPath(behaviorListHolder.nodeListAsset).LastIndexOf("/");
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