using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EffectController), typeof(ColorCycler))]
public class PsychObject : MonoBehaviour {

    public EffectController fxController {get; protected set;}
    public ColorCycler colorCycler { get; protected set; }

    public float defaultLife=100;
    public float currentLife;
    public float LifeRatio { get { return currentLife/defaultLife; } }
	// Use this for initialization
	void Start () {
        //GetComponentInChildren<()
        fxController = GetComponent<EffectController>();
        colorCycler = GetComponent<ColorCycler>();
        currentLife = defaultLife;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnValidate()
    {
        if (this.isActiveAndEnabled)
            currentLife = Mathf.Clamp(currentLife,0, defaultLife);
        else
            currentLife = defaultLife;

        if (!fxController)
            fxController=GetComponent<EffectController>();
        
        fxController.intensityModifier = LifeRatio;
    }
}
