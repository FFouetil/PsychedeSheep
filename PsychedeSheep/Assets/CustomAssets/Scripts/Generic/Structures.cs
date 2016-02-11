using UnityEngine;
using UnityEditor;

using System;
[Serializable]
public struct RangeF {
    
    public bool isValid { get { return max > min; } }
    public float min;
    public float max;
    public float delta { get { return max - min; } }
}
