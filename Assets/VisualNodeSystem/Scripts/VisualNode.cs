using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class VisualNodeBase : ScriptableObject{
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

[System.Serializable]
public class ANode : VisualNodeBase
{
    public int lala;
}

[System.Serializable]
public class BNode : VisualNodeBase
{
    public int lblb;
    public int c;
    public int d;
    public int e;

    public override int ChildMax()
    {
        return 2;
    }

#if UNITY_EDITOR
    public override string[] ChildrenLabels()
    {
        return new string[] { "F", "T" };
    }
#endif
}