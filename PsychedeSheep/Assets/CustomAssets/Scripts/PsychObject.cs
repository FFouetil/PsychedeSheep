using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EffectController), typeof(ColorCycler))]
public class PsychObject : MonoBehaviour {

    public EffectController fxController {get; protected set;}

    public float defaultLife=100;
    public float currentLife;
    public float overlifeLimitRatio = 3f;

    public float LifeRatio { get { return currentLife/defaultLife; } }
    public float OverLifeRatio { get { return (currentLife / overlifeLimitRatio); } }

	protected NavMeshAgent navAgent;
	public GameObject target;
    protected Dictionary<GameObject,bool> particleSrcFilter = new Dictionary<GameObject, bool>(64);

    // Use this for initialization
    void Start () {
        //GetComponentInChildren<()
        fxController = GetComponent<EffectController>();
		navAgent = GetComponent<NavMeshAgent>();
        currentLife = defaultLife;
        InitParticleFilter();
		target=FindObjectOfType<PlayerCharacter>().gameObject;
		navAgent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //test.
        float scale = 1f;
		var targetDist=Vector3.Distance(transform.position,target.transform.position);
		Debug.Log("Target dist: "+targetDist);
        //if life is below max ratio
		if (LifeRatio < overlifeLimitRatio && targetDist > 0.5f)
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
            PlayBlowParticles();


        }


    }

    public void PlayBlowParticles()
    {
        var scale = 1f;
        foreach (ParticleSystem ps in fxController.partSystems)
        {
            ps.transform.SetParent(null);
            ps.loop = false;

            int nbParts= 3000;
            int nbBatchs = 10;
            ps.maxParticles = nbParts / (int)scale;
            for (float i=1; i<= nbBatchs; ++i)
            {
                ps.startSize = LifeRatio * LifeRatio* scale;
                ps.startSpeed *= 6.666f * LifeRatio/ scale;
                ps.startLifetime *= 0.4f* scale;                
                
                ps.Emit(nbParts / nbBatchs);
                scale += i*0.5f;
            }

            Destroy(ps.gameObject, ps.startLifetime * 2);
            //ps.Play();
        }
        DestroyObject(this.gameObject);
    }

    void InitParticleFilter()
    {
        /*foreach (ParticleSystem ps in fxController.partSystems)
            particleSrcFilter.Add(ps.gameObject, true);
        !fxController.partSystems.Contains(other.GetComponentInChildren<ParticleSystem>()*/
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
        bool isFiltered;
        bool srcFound=particleSrcFilter.TryGetValue(other, out isFiltered);
        if (!srcFound)
        {
            //isFiltered = !other.GetComponentInParent<VacuumGun>();
            isFiltered = !other.name.StartsWith("Wp");
            Debug.Log("Filter " + other+": "+isFiltered);            
            particleSrcFilter.Add(other, isFiltered);
        }


        if (other != this && !isFiltered
            /*&& !fxController.partSystems.Contains(other.GetComponentInChildren<ParticleSystem>())*/)
        {

            //Debug.Log("PsychObj Received particle from " + other.name);
            //Debug.LogWarning("Doesn't filter particles from other objects yet!");
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
