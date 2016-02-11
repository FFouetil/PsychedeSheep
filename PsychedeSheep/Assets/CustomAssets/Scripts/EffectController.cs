using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectController : MonoBehaviour {
    protected static List<EffectController> fxControllers = new List<EffectController>(8);

    [SerializeField]
    public List<ParticleSystem> partSystems;

    public bool updateLinkedControllers=true;

    public ScaleMorpher scaleMorpher;
    public ColorCycler colorCycler;

    public Transform targetEntity;
    [Range(0, 100f)]
    public float nearEffectIntensity = 1f;
    [Range(0, 100f)]
    public float farEffectIntensity = 10f;

    public float nearEffectDistance=0.25f;
    public float farEffectDistance = 10f;
    [Range(0,100f)]
    public float effectDistanceModifier = 1f;

    [Range(0, 10f)]
    public float intensityModifier = 1f;



    // Use this for initialization
    void Start () {
        if (!targetEntity)
            targetEntity = GameObject.FindObjectOfType<Sheep>().transform;

        fxControllers.Add(this);
        GetComponentsInChildren<ParticleSystem>(partSystems);

    }
	
	// Update is called once per frame
	void Update () {

        //scales bounce duration according to distance
        var dist = Vector3.Distance(this.transform.position, targetEntity.position);
        var distRatio = MathHelper.ValueByDeltaToLinearRatio(nearEffectDistance, farEffectDistance, dist);
        //var distRatio = 
        scaleMorpher.morphCycleDuration = Mathf.Lerp(1/nearEffectIntensity,1/farEffectIntensity, distRatio) * effectDistanceModifier / intensityModifier;

        foreach (ParticleSystem ps in partSystems)
        {
            var color = ps.startColor;
            color = colorCycler.CurrentColor* intensityModifier;
            color.a +=0.5f;
            ps.startColor = color;
        }

        colorCycler.effectModifier = intensityModifier;

    }


    void OnValidate()
    {
        if (updateLinkedControllers)
        {
            Debug.Log("Syncing EffectControllers");
            foreach (EffectController fxc in fxControllers)
            {
                fxc.nearEffectIntensity = nearEffectIntensity;
                fxc.farEffectIntensity = farEffectIntensity;
                fxc.nearEffectDistance = nearEffectDistance;
                fxc.farEffectDistance = farEffectDistance;
                fxc.effectDistanceModifier = effectDistanceModifier;
            }
        }

    }

    /*
    void OnTriggerStay(Collider other)
    {
        var vac =other.GetComponentInParent<VacuumGun>();
        if ( vac && vac.isAspiring )
        {
            var distTV = Vector3.Distance(vac.transform.position, transform.position);
            Debug.Log("Collision Stay EC with" + other.name);
            foreach ( ParticleSystem ps in partSystems)
            {
                //var direction = vac.transform.position - transform.position;
                
                /*
                var forceModule = ps.forceOverLifetime;
                forceModule.enabled = true;
               
                forceModule.space = ParticleSystemSimulationSpace.World;
                forceModule.x = new ParticleSystem.MinMaxCurve(direction.x*10);
                forceModule.y = new ParticleSystem.MinMaxCurve(direction.y*10);
                forceModule.z = new ParticleSystem.MinMaxCurve(direction.z*10);
                

                int np=ps.GetParticles(p);
                Debug.Log("Collision Stay EC with" + other.name+ " - Nb Particles: "+np);
                for (int i = 0; i < p.Length; i++)
                {
                  //  p[i].
                   
                    p[i].position = Vector3.MoveTowards(
                        p[i].position, vac.transform.position, Time.deltaTime*vac.GetPowerAtRange(
                            Vector3.Distance(p[i].position, vac.transform.position)));
                    p[i].velocity *= 0.9f;
                    //p[i].lifetime *= 0.9f;
                    //p[i].position = Vector3.zero;

                }
                ps.SetParticles(p, np);
            }
        }
        //Debug.Log("Collision Stay EC "+other.name);
        
    }
    */
}
