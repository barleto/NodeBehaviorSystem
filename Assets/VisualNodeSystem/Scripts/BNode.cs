using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

