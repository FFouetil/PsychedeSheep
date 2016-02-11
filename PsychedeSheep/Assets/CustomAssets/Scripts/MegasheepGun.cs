using UnityEngine;
using System.Collections;

public class MegasheepGun : BaseGun {


    public float weaponChargeDuration;
    protected float weaponChargeTimer;
    protected float weaponChargeRatio;

	// Use this for initialization
	void Start () {
        weaponChargeTimer = 0;
        emitterRates = new ParticleSystem.MinMaxCurve[particleFx.Count];
        for (int p = 0; p < particleFx.Count; ++p)
        {
            particleFx[p].Stop();
            emitterRates[p] = particleFx[p].emission.rate;
        }

        if (!ProjectileSource)
            ProjectileSource = transform;

    }
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetButtonDown("Fire1"))
        {
            
            weaponChargeTimer = 0;
            Debug.Log("Pressing Fire1");
            PlayChargeParticles();
        }
        else if (Input.GetButton("Fire1"))
        {

            Debug.Log("Holding Fire1");
            ChargeWeapon();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Releasing Fire1");
            Fire();
            StopChargeParticles();
        }
    }

    void ChargeWeapon()
    {
        weaponChargeTimer += Time.deltaTime;
        weaponChargeRatio = (weaponChargeDuration > 0) ? weaponChargeTimer / weaponChargeDuration : 1f ;
        if (weaponChargeRatio <= 1f)
        {
            for (int p = 0; p < particleFx.Count; ++p)
            {
                //ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve(weaponChargeRatio* emitterRates[p].curveScalar);
                //particleFx[p].emission.rate.curveScalar = rate.curveScalar;
                var em=particleFx[p].emission;
                em.rate= new ParticleSystem.MinMaxCurve(2f*weaponChargeRatio* particleFx[p].maxParticles);

                //particleFx[p].particleEmitter.emission.rate = rate;
                particleFx[p].startColor=Color.white* weaponChargeRatio+Color.black;
            }
        }
        else
        {
            //do nothing?
            for (int p = 0; p < particleFx.Count; ++p)
            {
                particleFx[p].startColor = Color.white * weaponChargeRatio;
                particleFx[p].startSize = weaponChargeRatio;
            }
        }
    }

    void Fire()
    {
        
        var projObj=(GameObject)Instantiate(ProjectileType01.gameObject, ProjectileSource.transform.position, ProjectileSource.transform.rotation);
        if (projObj)
        {
            var projInst = projObj.GetComponent<Projectile>();
            if (projInst)
                projInst.Launch(1500, this.weaponChargeRatio);
        }


        weaponChargeTimer = 0;
    }

    void PlayChargeParticles()
    {
        for (int p = 0; p < particleFx.Count; ++p)
        {

            particleFx[p].playbackSpeed = 1f;
            particleFx[p].Play();
            //particleFx[p].Emit(1);
            //particleFx[p].Emit(particleFx[p].maxParticles);
        }
    }

    void StopChargeParticles()
    {
        for (int p = 0; p < particleFx.Count; ++p)
        {
            particleFx[p].playbackSpeed *= 5f;
            particleFx[p].Stop();
        }
    }

}
