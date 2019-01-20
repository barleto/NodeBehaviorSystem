using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class VisualNodeRoot : ScriptableObject {
    public VisualNodeBase root;
    [HideInInspector]
    public List<VisualNodeBase> nodes = new List<VisualNodeBase>();
}
