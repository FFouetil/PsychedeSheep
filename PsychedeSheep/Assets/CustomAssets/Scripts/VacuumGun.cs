using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Candlelight;

public class VacuumGun : BaseGun
{
    public enum AspirationStatus
    {
        Stopped,
        PoweringUp,
        PoweringDown,
        FullyPowerd,
    }
    public AspirationStatus aspirationStatus= AspirationStatus.Stopped;

    //
    //Attributes
    //

    public SphereCollider col;

    [Header("Aspiration settings")]
    public float aspirationPower = 10f;
    public float maxRange = 1f;
    [Range(0,359.99f),SerializeField]
    protected float coneAngleDeg = 90; 
    public bool isAspiring;
    public bool isBlowing;
    ///<summary>Half-angle of the cone</summary>
    public float LateralAngleDeg { get { return coneAngleDeg / 2; } set { coneAngleDeg = Mathf.Clamp(value * 2, 0, 359.99f); } }
    ///<summary>Normal of the maximum cone angle</summary>
    public float EffectiveNormal { get { return MathHelper.AngleDegToNormal(LateralAngleDeg); } }

    [Header("Blow settings")]
    public float storageLimit=1000;
    [SerializeField,PropertyBackingField("StorageLevel")]
    private float _storageLevel;
    public float StorageLevel { get { return _storageLevel; } set { _storageLevel = Mathf.Clamp(value,0,storageLimit); } }
    [SerializeField]
    public List<ParticleSystem.Particle> storedParticles;
    public ParticleSystem blowFxMain;


    //cached stuff
    [Space]
    protected List<ParticleSystem> fxPartSystems;
    protected ParticleSystem.Particle[] p = new ParticleSystem.Particle[2500];

    // Use this for initialization
    void Awake()
    {
        storedParticles = new List<ParticleSystem.Particle>((int)(storageLimit * 100));
        col = GetComponentInChildren<SphereCollider>();
        if (col)
            col.isTrigger = false;
    }
    void Start()
    {
        _storageLevel = 0;
        if (col)
        {
            col.radius = maxRange * 20f;
            col.isTrigger = true;
            col.enabled = true;

        }

    }

    // Update is called once per frame
    void Update()
    {
        //fire 2 is aspiration key
        if (Input.GetButtonDown("Fire2"))
        {
            aspirationStatus = AspirationStatus.PoweringUp;
            isAspiring = true;
            Debug.Log("Pressing Fire2");
            PlayAspirationParticles();
        }
        else if (Input.GetButton("Fire2"))
        {

            //Debug.Log("Holding Fire2");
            //Aspirate();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            aspirationStatus = AspirationStatus.PoweringDown;
            isAspiring = false;
            Debug.Log("Releasing Fire2");
            StopAspirationParticles();
            //Fire();
        }

        //fire 1 is shoot key
        else if (Input.GetButtonDown("Fire1"))
        {
            isBlowing = true;
            blowFxMain.Play();
            Debug.Log("Pressing Fire1");
            PlayAspirationParticles();
        }
        else if (Input.GetButton("Fire1"))
        {
            
            //Debug.Log("Holding Fire1");
            //Aspirate();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            isBlowing = false;
            blowFxMain.Stop();
            Debug.Log("Releasing Fire1");

        }

        if (isBlowing)
            Fire();

    }


    void Fire()
    {
        blowFxMain.Pause();
        if (storedParticles.Count > 0)
        {
            int bunchSize = Mathf.Min(4, storedParticles.Count);            

            var nextParticles = storedParticles.GetRange(0, bunchSize);
            float size=0, life=0;
            Vector4 color = new Vector4(0,0,0,0);

            foreach (ParticleSystem.Particle p in nextParticles)
            {
                size += p.startSize;
                life += p.startLifetime;
                var c = (Color)p.startColor;
                color += new Vector4(c.r, c.g, c.b, c.a);    
            }

            blowFxMain.startSize = size/bunchSize;
            blowFxMain.startLifetime = life/bunchSize;
            blowFxMain.startColor =  color / bunchSize;
            
            storedParticles.RemoveRange(0,bunchSize);
        }
        else
        {
            blowFxMain.startColor = Color.gray/5;
        }
        _storageLevel = storedParticles.Count;
        blowFxMain.Play();

    }

    void StopAspirationParticles()
    {
        throw new NotImplementedException("StopAspirationParticles");
    }

    void Aspirate(Collider other)
    {

        var psychObj = other.GetComponentInParent<PsychObject>();
        var fxCtrl = psychObj ? psychObj.fxController : null;

        if (psychObj) {
            AspiratePsychObj(psychObj);
        }
        //here, we take particle effects and aspire them toward the weapon
        if (fxCtrl)
        {
            AspirateParticles(fxCtrl);    
        }

    }

    void AspiratePsychObj(PsychObject psychObj)
    {
        var dist = Vector3.Distance(psychObj.transform.position.normalized, col.transform.position.normalized);
        var angularPower = MathHelper.ValueByDeltaToLinearRatio(
                EffectiveNormal, 1f,
                Vector3.Dot(
                    transform.forward.normalized, (transform.position - psychObj.transform.position).normalized));
        //Debug.Log("AspiratePsychObj " + psychObj.name);
        var absorbedLife= Time.deltaTime
            * GetPowerAtRange(dist)
            * angularPower;
        psychObj.currentLife -= absorbedLife;

        StorageLevel += absorbedLife;

        psychObj.currentLife = Mathf.Max(0, psychObj.currentLife);
        psychObj.fxController.intensityModifier = psychObj.LifeRatio;
    }

    void AspirateParticles(EffectController fxCtrl)
    {
        var distTV = Vector3.Distance(fxCtrl.transform.position, col.transform.position);

        //Debug.Log("Collision Stay Vac with " + other.name);
        foreach (ParticleSystem ps in fxCtrl.partSystems)
        {

            /*
            var forceModule = ps.forceOverLifetime;
            forceModule.enabled = true;

            forceModule.space = ParticleSystemSimulationSpace.World;
            forceModule.x = new ParticleSystem.MinMaxCurve(direction.x*10);
            forceModule.y = new ParticleSystem.MinMaxCurve(direction.y*10);
            forceModule.z = new ParticleSystem.MinMaxCurve(direction.z*10);
            */

            int np = ps.GetParticles(p);
            //Debug.Log("Collision Stay Vac with " + other.name + " - Nb Particles: " + np);
            for (int i = 0; i < np; i++)
            {
                var vel = p[i].velocity;
                if (i%2 == 0)
                {
                    
                    //calculate the effectiveness of aspiration according to distance and angle
                    var distP = Vector3.Distance(p[i].position.normalized, col.transform.position.normalized);
                    var dirNormal = Vector3.Dot(col.transform.forward.normalized, (col.transform.position - p[i].position).normalized);
                    var angleRatio = MathHelper.ValueByDeltaToLinearRatio(EffectiveNormal, 1f, dirNormal);
                    var distRatio=GetPowerAtRange(distP);
                    var effectRatio = angleRatio* angleRatio * distRatio;

                    //store every Nth particle if close to the gun
                    if (distP < 0.15f && i % 10 == 0)
                    {
                        StoreParticle(p[i]);
                        p[i]=new ParticleSystem.Particle();
                    }
                    else //aspire it toward the gun and add some gravity/momentum
                    {
                        /*if (i == 1)
                            Debug.Log("distP: " + distP);*/

                        p[i].position = Vector3.MoveTowards(
                            p[i].position, col.transform.position, effectRatio * Time.deltaTime);

                        vel.x = (col.transform.position.x- p[i].position.x) * Time.deltaTime * p[i].startSize*15;
                        vel.z = (col.transform.position.z- p[i].position.z ) * Time.deltaTime*p[i].startSize*15;
                        vel.y = Physics.gravity.y * Mathf.Clamp01(1 - effectRatio) * Time.deltaTime;
                        p[i].velocity = vel;
                    }

                }
                /*else
                {
                    vel.y += Physics.gravity.y * 0.005f;
                    p[i].startSize *= 0.8f;
                }*/


                /*
                if (i == 0)
                    Debug.Log("angle: " + angleRatio);*/
            }
            ps.SetParticles(p, np);
        }
    }

    private void StoreParticle(ParticleSystem.Particle particle)
    {
        //Debug.Log("Storing particule in Vacuum");
        
        storedParticles.Add(particle);
        _storageLevel = storedParticles.Count;
    }


    void PlayAspirationParticles()
    {
        throw new NotImplementedException("PlayAspirationParticles");
    }

    public float GetPowerAtRange(float distance)
    {
        return Mathf.SmoothStep(aspirationPower, 0f, distance / maxRange);
    }

    void OnTriggerStay(Collider other)
    {
        //if aspiration is enabled
        if (isAspiring)
            Aspirate(other);
        
    }

    void OnValidate()
    {
        if (col)
            col.radius = maxRange * 20f;
    }
}