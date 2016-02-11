using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EffectController), typeof(ColorCycler))]
public class PsychObject : MonoBehaviour {

    public EffectController fxController {get; protected set;}

    public float defaultLife=100;
    public float currentLife;
    public float overlifeLimitRatio = 1.5f;

    public float LifeRatio { get { return currentLife/defaultLife; } }
    public float OverLifeRatio { get { return (currentLife / overlifeLimitRatio); } }

    // Use this for initialization
    void Start () {
        //GetComponentInChildren<()
        fxController = GetComponent<EffectController>();
        currentLife = defaultLife;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //test.
        float scale=1f;
        if (LifeRatio > 1)
        {
            scale = Mathf.SmoothStep(1f, overlifeLimitRatio, OverLifeRatio);
        }
        else
        {
            scale = Mathf.SmoothStep(0.5f, 1f, LifeRatio);
        }
        
        fxController.scaleMorpher.globalScaleModifier = scale;
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

    void OnParticleCollision(GameObject other)
    {
        //ugly!
        
        if (other && !fxController.partSystems.Contains(other.GetComponentInChildren<ParticleSystem>()) )
        {
            Debug.Log("PsychObj Received particle from " + other.name);
            currentLife += 1f;
            fxController.intensityModifier = LifeRatio;
            /*var vac = other.transform.parent.parent.parent.GetComponentInChildren<VacuumGun>();
            if (vac)
            {
                currentLife += 0.1f;
                
            }*/
        }


    }
}
