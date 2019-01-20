using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class VisualNodeBase : ScriptableObject{
    [SerializeField]
    [HideInInspector]
    public List<VisualNodeBase> children = new List<VisualNodeBase>();
    [HideInInspector]
    public VisualNodeBase parent;
#if UNITY_EDITOR
    [HideInInspector]
    public Rect windowPosition;

    public virtual string[] ChildrenLabels()
    {
        return new string[] {"+"};
    }
#endif

    public virtual int ChildMax()
    {
        return 1;
    }

    public void VerifyChildrenVectorSize()
    {
        while (children.Count() < ChildMax())
        {
            children.Add(null);
        }
    }
}