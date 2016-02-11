using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class BaseGun : MonoBehaviour {

    protected ParticleSystem.MinMaxCurve[] emitterRates;

    public Transform ProjectileSource;
    public Projectile ProjectileType01;

    public List<ParticleSystem> particleFx;

}
