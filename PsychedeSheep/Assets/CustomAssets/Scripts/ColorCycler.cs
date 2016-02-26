using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
public class ColorCycler : MonoBehaviour, ITimed {

    public Color CurrentColor { get; protected set; }
	public List<Color> colorCycle;
    [Range(0, 3)]
    public float selfIllumRatioMin=0.5f;
    [Range(0, 3)]
    public float selfIllumRatioMax = 1.5f;
    public float colorChangeDuration=1;
    [Range(0, 10f)]
    public float effectModifier = 1f;

    [Range(1,64)]
    public int fadeSpeed= 8;
    public MonoBehaviour linkedTimedModule;
    [SerializeField]
    public bool matchScaleBounceCycles;    
    protected float colorChangeTimer=0;

    protected Renderer r;
    private bool updateInEditor=false;

    /// <summary>Returns color change cycle duration</summary>
    public float TimerDuration  { get {  return colorChangeDuration; } }

    void Awake()
    {
        r = GetComponent<Renderer>();

    }
    // Use this for initialization
    void Start () {

		if (colorCycle == null){
			colorCycle=new List<Color>(2);
			colorCycle.Add(Color.white);
		}
        /*if (colorCycle.Count == 1)
        {   //makes sure we never try to cycle through un-initialized 2nd slot
            colorCycle[1] = colorCycle[0];
        }*/
        if (!r)
            r = GetComponent<Renderer>();
        r.sharedMaterial.color=(colorCycle[0]);
        r.sharedMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        //r.material.color= colorCycle[0] * selfIllumRatio;
        r.sharedMaterial.SetColor("_Albedo", colorCycle[0]);
        r.sharedMaterial.SetColor("_EmissionColor", colorCycle[0]);
        r.sharedMaterial.SetFloat("_EmissionLM", 0.357f);
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
        //duration /= effectModifier;
        colorChangeTimer += Time.deltaTime;

        var timerRatio = colorChangeTimer / duration;
        //if timer reaches duration, reset timer and do some other tasks 
        if (timerRatio > 1)
        {
            colorChangeTimer = 0;

            NextColor();

        }
        else
        {
            UpdateMaterial(timerRatio);
        }

  
    }

    void NextColor()
    {
		if (colorCycle.Count > 1){
			//remove current/first color from the list and put it back at the end
	        var queuedColor = colorCycle[0];
	        colorCycle.RemoveAt(0);
	        colorCycle.Add(queuedColor);
		}
    }

    void UpdateMaterial(float timerRatio)
    {
        float smoothedInterp = MathEx.EaseOut(0, 1, timerRatio);
        //var smoothed = MathHelper.SmoothExpFadeOut(timerRatio,fadeSpeed);
        Color color = (colorCycle.Count > 1) ? Color.Lerp(colorCycle[0], colorCycle[1], smoothedInterp) * effectModifier : colorCycle[0];

        r.material.color = color;
        color.a = effectModifier;
        CurrentColor = color;
        //if (selfIllumRatio > 0)
        r.material.color = color * effectModifier;
        //r.sharedMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        r.material.SetColor("_Albedo", color * effectModifier);
        var illum= Mathf.Lerp(selfIllumRatioMin, selfIllumRatioMax, MathEx.Phase01(timerRatio));
        r.material.SetColor("_EmissionColor", color*  effectModifier* illum);
        r.material.SetFloat("_EmissionLM", illum * effectModifier);
        
    }

    void OnValidate()
    {
        if (!r)
            r = GetComponent<Renderer>();
        if (updateInEditor)
            UpdateMaterial(0.5f);
    }
}
