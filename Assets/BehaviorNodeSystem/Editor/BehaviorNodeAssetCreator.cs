using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

namespace BehaviorNodePlugin
{

    public class BehaviorNodeAssetCreator
    {

        public string rootPath;

        private const string nodeListNameTag = "BehaviourNodeList";
        private const string nodeNameTag = "Node";
        public BehaviorNodeAssetCreator(string root)
        {
            this.rootPath = root;
        }

        public BehaviorNodesList CreateNodeListAsset()
        {
            int numberedSufix = -1;
            string folderFinalName;
            do
            {
                numberedSufix++;
                folderFinalName = rootPath + "/" + nodeListNameTag + numberedSufix;
            } while (AssetDatabase.IsValidFolder(folderFinalName));
            AssetDatabase.CreateFolder(rootPath, nodeListNameTag + numberedSufix);
            var newAsset = ScriptableObject.CreateInstance<BehaviorNodesList>();
            var assetName = folderFinalName + "/" + nodeListNameTag + numberedSufix + ".asset";
            try
            {
                AssetDatabase.CreateAsset(newAsset, assetName);
            }
            catch (Exception e)
            {
                Debug.LogError("Could not create NodesList asset at path " + "Assets/" + assetName);
                return null;
            }
            return newAsset;
        }

        public BehaviorNode CreateNodeAsset(BehaviorNodesList list, System.Type type)
        {
            var path = AssetDatabase.GetAssetPath(list);
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
            return (BehaviorNode)newNode;
        }
    }
}