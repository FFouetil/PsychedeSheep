using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
public class ColorCycler : MonoBehaviour, ITimed {


    public List<Color> colorCycle = new List<Color>(2);
    [Range(0, 1)]
    public float selfIllumRatio=0.5f;
    
    public float colorChangeDuration=1;

    [Range(1,64)]
    public int fadeSpeed= 8;
    public MonoBehaviour linkedTimedModule;
    [SerializeField]
    public bool matchScaleBounceCycles;    
    protected float colorChangeTimer;

    protected Renderer r;

    /// <summary>Returns color change cycle duration</summary>
    public float TimerDuration  { get {  return colorChangeDuration; } }

    // Use this for initialization
    void Start () {
        r = GetComponent<Renderer>();
        r.material.color=(colorCycle[0]);
        r.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        r.material.SetColor("_EmissionColor", colorCycle[0]* selfIllumRatio);

        //r.receiveShadows = false;

        if (linkedTimedModule != null)
        {
            if (linkedTimedModule is ITimed)
                matchScaleBounceCycles = true;
            else
                linkedTimedModule = null;
        }
        
    }
	
	// Update is called once per frame
	void Update () {

        //set used duration to ScaleBounce duration if present and matching is enabled, else use local value
        float duration = (matchScaleBounceCycles && linkedTimedModule is ITimed) ? ((ITimed)linkedTimedModule).TimerDuration : colorChangeDuration;
        colorChangeTimer += Time.deltaTime;

        var timerRatio = colorChangeTimer / duration;
        //if timer reaches duration, reset timer and do some other tasks 
        if (timerRatio > 1)
        {
            colorChangeTimer = 0;
            //remove current/first color from the list and put it back at the end
            var queuedColor = colorCycle[0];
            colorCycle.RemoveAt(0);
            colorCycle.Add(queuedColor);
        }
        else
        {
            var smoothed = MathHelper.SmoothExpFadeIn(timerRatio,fadeSpeed);
            var color = Color.Lerp(colorCycle[0], colorCycle[1], smoothed);
            r.material.color = color;
            color.a = selfIllumRatio;
            //if (selfIllumRatio > 0)
            r.material.SetColor("_EmissionColor", color* selfIllumRatio);
            

        }

  
    }
}
