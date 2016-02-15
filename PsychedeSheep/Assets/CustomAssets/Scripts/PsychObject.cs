using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EffectController), typeof(ColorCycler))]
public class PsychObject : MonoBehaviour {

    public EffectController fxController {get; protected set;}

    public float defaultLife=100;
    public float currentLife;
    public float overlifeLimitRatio = 3f;

    public float LifeRatio { get { return currentLife/defaultLife; } }
    public float OverLifeRatio { get { return (currentLife / overlifeLimitRatio); } }

    // Use this for initialization
    void Start () {
        //GetComponentInChildren<()
        fxController = GetComponent<EffectController>();
        currentLife = defaultLife;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //test.
        float scale = 1f;
        //if life is below max ratio
        if (LifeRatio < overlifeLimitRatio)
        {
            if (LifeRatio <= 1f)
                scale = Mathf.LerpUnclamped(0.5f, 1f, LifeRatio);
            else
                scale *= LifeRatio * LifeRatio;// Mathf.LerpUnclamped(1f, 1f, LifeRatio);

            foreach (ParticleSystem ps in fxController.partSystems)
            {
                ps.startSize = scale;
            }


            fxController.scaleMorpher.globalScaleModifier = scale;

        }
        else //if light is over max ratio
        {
            //scale = 0.1f;//MathHelper.EaseOut(LifeRatio, LifeRatio, LifeRatio);

            //blow it up
            foreach (ParticleSystem ps in fxController.partSystems)
            {
                ps.transform.SetParent(null);
                ps.startSize = LifeRatio * LifeRatio;
                ps.startSpeed *= 10 * LifeRatio;
                ps.startLifetime *= 0.75f;
                ps.loop = false;
                
                ps.Emit(1000);
                Destroy(ps.gameObject, ps.startLifetime*2);
                //ps.Play();
            }
            DestroyObject(this.gameObject);

        }


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
            Debug.LogWarning("Doesn't filter particles from other objects yet!");
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
