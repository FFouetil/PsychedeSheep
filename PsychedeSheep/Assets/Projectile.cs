using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float LaunchForce = 100;

	Rigidbody rigidBody;
	bool alreadyLaunched=false;
	// Use this for initialization
	void Start () {
		if ( !rigidBody ){
			rigidBody = GetComponent<Rigidbody>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetButtonDown("Fire1") && !alreadyLaunched ){
			Launch(LaunchForce);
		}
	}

	internal void Launch(float force){
		rigidBody.AddRelativeForce(force,0,0);
	}
}
