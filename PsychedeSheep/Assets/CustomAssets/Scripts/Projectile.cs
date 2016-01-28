using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float LaunchForce = 100;
    public float TimeToLive = 3f;
    public float Size = 1f;
    protected float timeElapsed;

	Rigidbody rigidBody;
	bool alreadyLaunched=false;
	// Use this for initialization
	void Awake () {
		if ( !rigidBody )
		    rigidBody = GetComponent<Rigidbody>();
		

         Destroy(this.gameObject, TimeToLive);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        /*
        //test line
		if ( Input.GetButtonDown("Fire1") && !alreadyLaunched ){
			Launch(LaunchForce);
		}
        */


    }

	internal void Launch(float force, float size=1f){
        transform.localScale = Vector3.one * Mathf.Max(0.25f, size);
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.startSize *= size*1.25f;
            ps.startLifetime *= size;
        }
        /*if (!rigidBody)
            rigidBody = GetComponent<Rigidbody>();*/
        rigidBody.AddRelativeForce(0,0,force,ForceMode.Force);
	}
}
