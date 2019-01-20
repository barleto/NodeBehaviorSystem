using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class VisualNodeSystemConfiguration : ScriptableObject {

    [SerializeField] private string _pathToSaveNodeLists = "Assets/VisualNodeSystem/Saves/";
    static VisualNodeSystemConfiguration _config;

    public static string GetPathToSaveNodeLists()
    {
        return GetConfig()._pathToSaveNodeLists;
    }

    static VisualNodeSystemConfiguration GetConfig()
    {
        if (_config != null)
        {
            return _config;
        }
        _config = Resources.Load<VisualNodeSystemConfiguration>("VisualNodeSystemConfiguration");
        return _config;
    }
}
