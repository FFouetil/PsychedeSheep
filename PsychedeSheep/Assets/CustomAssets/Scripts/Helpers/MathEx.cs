using System;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
#endif


public static class MathEx
{
    public static readonly float Deg2Rad = 0.0174532924f;
    public static readonly float Rad2Deg = 57.29578f;
    
    public static float Clamp(float min, float max, float value)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public static float Lerp(float a, float b, float t)
    {
        t = Clamp(0, 1, t);
        return (1 - t) * a + t * b;
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    /// <summary>Similar to smoothstep, but smoother at extremities and steeper slope</summary>
    public static float SmootherStep(float a, float b, float t)
    {
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        return Mathf.Lerp(a, b, t);
    }
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
    public static float BounceNegative(float ratio)
    {
        return Mathf.Sin(ratio * Mathf.PI + Mathf.PI);
    }

    /// <summary>Bounce effect, 0->-1->0 pattern</summary>
    public static float BouncePositive(float ratio)
    {
        return Mathf.Sin(ratio * Mathf.PI);
    }

    /// <summary>Smooth exponential fade</summary>
    public static float SmoothExpFadeOut(float ratio, int pow)
    {  
        return Mathf.Pow(Mathf.SmoothStep(0, 1, ratio), (float)(pow));

    }
    /// <summary>Smooth inverse exponential fade. Needs more testing!</summary>
    public static float SmoothExpFadeIn(float ratio, int pow)
    {
        
        return Mathf.Pow(1-Mathf.SmoothStep(0,1, 1-ratio), (float)(pow));
    }

    /// <summary>Gives the 0..1 ratio of value between min and max</summary>
    public static float ValueByDeltaToLinearRatio(float min, float max, float value)
    {
        //return a hard value or fear the mighty division by zero
        if (min == max)
            return float.NaN;

        if (min > max)
        {
            var t = min;
            min = max;
            max = t;
        }
        
        var delta = Math.Abs(max - min);
        var offset = min;
        //min = 0; max -= offset; value -= offset;
        return (Mathf.Clamp(value,min, max) - min) / delta;
    }

    /// <summary>Converts a lateral angle (0 to 180) in degree to a normal value [-1,1]</summary>
    public static float AngleDegToNormal(float angleDeg)
    {        
        
        return Mathf.Cos(Mathf.Deg2Rad*angleDeg);
    }

    /// <summary>Converts a normal value [-1,1] to a lateral angle (0 to 180) in degree </summary>
    public static float AngleRadToNormal(float angleRad)
    {
        return Mathf.Cos(angleRad);
    }

    /// <summary>Interpolation with smoothing at the end</summary>
    public static float EaseOut(float a, float b, float t)
    {
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
        return Mathf.Lerp(a, b, t);
    }

    /// <summary>Interpolation with smoothing at the beginning</summary>
    public static float EaseIn(float a, float b, float t)
    {
        t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        return Mathf.Lerp(a, b, t);
    }
#else


    public static float LerpUnclamped(float a, float b, float t)
    {
        return (1 - t) * a + t * b;
    }

    public static float SmoothStep(float a, float b, float t){
        t = t * t * (3f - 2f * t);
        return Lerp(a, b, t);
    }

    public static float SmootherStep(float a, float b, float t)
    {
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        return Lerp(a, b, t);
    }

    public static float PhaseNegPos(float ratio)
    {
        return (float) Math.Sin(ratio * Math.PI*2);
    }

    public static float Phase01(float ratio)
    {
        return (float) Math.Sin(ratio * Math.PI * 2d)/2+0.5f;
    }

    public static float EaseOut(float a, float b, float t)
    {
        t = (float)Math.Sin(t * Math.PI * 0.5);
        return Lerp(a,b,t);
    }

    public static float EaseIn(float a, float b, float t)
    {
        t = (float)(1f - Math.Cos(t * Math.PI * 0.5));
        return Lerp(a, b, t);
    }

        public static float ValueByDeltaToLinearRatio(float min, float max, float value)
    {
        //return a hard value or fear the mighty division by zero
        if (min == max)
            return float.NaN;

        if (min > max)
        {
            var t = min;
            min = max;
            max = t;
        }
        
        var delta = Math.Abs(max - min);
        var offset = min;
        //min = 0; max -= offset; value -= offset;
        return (Clamp(value,min, max) - min) / delta;
    }


#endif


}
