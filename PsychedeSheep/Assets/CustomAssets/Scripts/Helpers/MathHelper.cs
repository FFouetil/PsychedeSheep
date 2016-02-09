using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEditor;
using UnityEngine;
#endif


public static class MathHelper
{
#if UNITY_EDITOR || UNITY_STANDALONE

    /// <summary>Sin phases. 1 to -1 amplitude</summary>
    public static float PhaseNegPos(float ratio)
    {
        return Mathf.Sin(ratio * Mathf.PI*2f);
    }

    /// <summary>Sin phases. adjusted to be between 0 and 1</summary>
    public static float Phase01(float ratio)
    {
        return Mathf.Sin(ratio * Mathf.PI * 2f)/2f+0.5f;
    }

    /// <summary>Bounce effect, 0->1->0 pattern</summary>
    public static float Bounce(float ratio)
    {
        return Mathf.Sin(ratio * Mathf.PI + Mathf.PI);
    }

    /// <summary>Bounce effect, 0->-1->0 pattern</summary>
    public static float ReverseBounce(float ratio)
    {
        return Mathf.Sin(ratio * Mathf.PI);
    }

    /// <summary>Smooth exponential fade</summary>
    public static float SmoothExpFadeOut(float ratio, int pow)
    {  
        return Mathf.Pow(Mathf.SmoothStep(0, 1, ratio), (float)(pow));

    }
    /// <summary>Smooth exponential fade</summary>
    public static float SmoothExpFadeIn(float ratio, int pow)
    {
        return Mathf.Pow(1-Mathf.SmoothStep(0,1, 1-ratio), (float)(pow));
    }

    public static float DeltaValueToLinearRatio(float min, float max, float value)
    {
        //return a hard value or fear the mighty division by zero
        if (min == max)
            return 1;
        
        /*
        if (min > max)
        {
            var t = min;
            min = max;
            max = t;
        }*/
        
        var delta = Math.Abs(max - min);
        var offset = min;
        //min = 0; max -= offset; value -= offset;
        return (value- min) / delta;
    }
#else
        public static float PhaseNegPos(float ratio)
    {
        return (float) Math.Sin(ratio * Math.PI*2);
    }

    public static float Phase01(float ratio)
    {
        return (float) Math.Sin(ratio * Math.PI * 2d)/2+0.5f;
    }

#endif


}
