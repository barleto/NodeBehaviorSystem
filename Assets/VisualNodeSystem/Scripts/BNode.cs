using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BNode : VisualNodeBase
{
    public bool flag;

    public override int ChildMax()
    {
        return 2;
    }

#if UNITY_EDITOR
    public override string[] ChildrenLabelsInEditor()
    {
        return new string[] { "F", "T" };
    }

    public override Color[] LineColor()
    {
        return new Color[] { Color.red, Color.green, Color.green };
    }
#endif
}

