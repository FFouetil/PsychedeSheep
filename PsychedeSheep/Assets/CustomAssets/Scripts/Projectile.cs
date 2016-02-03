using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{

    public float LaunchForce = 100;
    public float TimeToLive = 3f;
    public float Size = 1f;

    protected float totalElapsedTime;
    protected float fadeoutElapsedTime;
    protected Rigidbody rigidBody;
    protected List<ParticleSystem> particleSystems=new List<ParticleSystem>(3);

    bool alreadyLaunched = false;

    // Use this for initialization
    void Awake()
    {
        if (!rigidBody)
            rigidBody = GetComponent<Rigidbody>();

        if (particleSystems.Count == 0)
            GetComponentsInChildren<ParticleSystem>(particleSystems);

        Destroy(this.gameObject, TimeToLive);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        //test line
        if ( Input.GetButtonDown("Fire1") && !alreadyLaunched ){
            Launch(LaunchForce);
        }
        */


    }

    internal void Launch(float force, float size = 1f)
    {
        transform.localScale = Vector3.one * Mathf.Max(0.25f, size);
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.startSize *= size * 1.25f;
            ps.startLifetime *= size;
        }
        /*if (!rigidBody)
            rigidBody = GetComponent<Rigidbody>();*/
        rigidBody.AddRelativeForce(0, 0, force, ForceMode.Force);
    }
}
