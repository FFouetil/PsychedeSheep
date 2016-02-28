using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EffectController), typeof(ColorCycler))]
public class PsychEnemy : MonoBehaviour {

	protected Collider col;
    public EffectController fxController {get; protected set;}
	public AudioSource sndSource;
	public AudioClip sndExplo;
	public AudioClip sndPouic;

    public float defaultLife=100;
    public float currentLife;
    public float overlifeLimitRatio = 3f;

    public float LifeRatio { get { return currentLife/defaultLife; } }
    public float OverLifeRatio { get { return (currentLife / overlifeLimitRatio); } }

	protected NavMeshAgent navAgent;
	public GameObject target;
    protected Dictionary<GameObject,bool> particleSrcFilter = new Dictionary<GameObject, bool>(64);

	protected float lastRaycast=0f;

	protected Vector3 spawnLocation;
	protected Vector3 lastPouicLocation;

    // Use this for initialization
    void Awake () {
        //GetComponentInChildren<()
        fxController = GetComponent<EffectController>();
		navAgent = GetComponent<NavMeshAgent>();
        currentLife = defaultLife;
        InitParticleFilter();
		target=FindObjectOfType<PlayerCharacter>().gameObject;
		navAgent.SetDestination(target.transform.position);

		//sndExplo=Resources.Load<AudioClip>("explo1.wav");
		sndSource=GetComponentInChildren<AudioSource>();
		sndSource.clip=sndExplo;
		spawnLocation=transform.position;
		lastPouicLocation=transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //test.
        float scale = 1f;
		//var targetDist=Vector3.Distance(transform.position,target.transform.position);
		//Debug.Log("Target dist: "+targetDist);
        //if life is below max ratio
		if (LifeRatio < overlifeLimitRatio )
        {
			if (fxController.scaleMorpher.IsStartingPhase && Vector3.Distance(transform.position,lastPouicLocation) > 0.75f){
				sndSource.pitch=Random.Range(0.9f,1.1f);
				sndSource.PlayOneShot(sndPouic);
				lastPouicLocation=transform.position;
			}

			if (LifeRatio <= 1f){
				scale = Mathf.LerpUnclamped(0.5f, 1f, LifeRatio);
				currentLife+=0.5f;
			}
                
            else
                scale *= LifeRatio * LifeRatio;// Mathf.LerpUnclamped(1f, 1f, LifeRatio);

            foreach (ParticleSystem ps in fxController.partSystems)
            {
                ps.startSize = scale;
            }

            fxController.scaleMorpher.globalScaleModifier = scale;
			navAgent.speed=Mathf.SmoothStep(1,9,LifeRatio);

			//if (false) //skip raycasts for testing
			if ( (lastRaycast+=Time.fixedDeltaTime) > 0.333f ){
				//they go back their spawn position unless they can see and follow him
				RaycastHit hit;
				var rayDirection = target.transform.position - transform.position;
				if (Physics.Raycast (transform.position,rayDirection, out hit)) {
					if (hit.transform == target.transform) {
						// enemy can see the player!
						navAgent.SetDestination(target.transform.position);
					} else {
						// there is something obstructing the view
						navAgent.SetDestination(spawnLocation);
					}
				}
				lastRaycast=0;

			}

        }
        else //if light is over max ratio
        {
            //scale = 0.1f;//MathHelper.EaseOut(LifeRatio, LifeRatio, LifeRatio);

            //blow it up
			Explode();


        }


    }

	public void Explode(){
		PlayExploSound();
		PlayBlowParticles();
		DestroyObject(this.gameObject);
	}

	public void PlayExploSound(){
		sndSource.transform.SetParent(null,true);
		sndSource.pitch=Random.Range(0.85f,1.15f);
		sndSource.Stop();
		sndSource.PlayOneShot(sndExplo);
		Destroy(sndSource.gameObject,sndExplo.length*1.25f);
	}

    public void PlayBlowParticles()
    {
        var scale = 1f;
        foreach (ParticleSystem ps in fxController.partSystems)
        {
            ps.transform.SetParent(null);
            ps.loop = false;

            int nbParts= 1500;
            int nbBatchs = 10;
            ps.maxParticles += nbParts / (int)scale;
            for (float i=1; i<= nbBatchs; ++i)
            {
                ps.startSize = LifeRatio * LifeRatio* scale;
                ps.startSpeed *= 6.666f * LifeRatio/ scale;
                ps.startLifetime *= 0.4f* scale;                
                
                ps.Emit(nbParts / nbBatchs);
                scale += i*0.4f;
				scale=Mathf.Min(scale,5f);
            }

            //Destroy(ps.gameObject, ps.startLifetime * 2);
            //ps.Play();
        }
        
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
            currentLife -= 5f;
            fxController.intensityModifier = LifeRatio;
            /*var vac = other.transform.parent.parent.parent.GetComponentInChildren<VacuumGun>();
            if (vac)
            {
                currentLife += 0.1f;
                
            }*/
        }


    }

	void OnCollisionEnter(Collision collision){
		if (collision.transform.GetComponent<PlayerCharacter>() ){
			Explode();
		}
	}
}
