using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

	protected Rigidbody rigid;
//	public ConstantForce JetPack;
//
//	public float MaxJetForce=500;
//	public float MaxEffectiveHeight=100;
//	[Range(0,1)]
//	public float FullPowerHeightRatio=0.875f;
//	public float TimeToFullspeed=5;
	protected float pressDuration;
	protected float lastRefForce;

	public float health=100;
	public bool canMove;

	public float baseSpeed=10f;
	// Use this for initialization
	void Start () {
		rigid=GetComponent<Rigidbody>();
		rigid.WakeUp();
		rigid.angularDrag=0.05f;

		//rigid.AddForce(0,1,0);

	}

	// Update is called once per frame
	void FixedUpdate () {
		rigid.drag=0;

		var v = Input.GetAxis("Vertical");
		var h = Input.GetAxis("Horizontal");
		var translateMod = 10f;
		var rY = Input.GetAxis("Horizontal2");
		var rotateMod = 10f*10f;

		if ( canMove )
		{
			transform.position += (rigid.transform.forward * Time.fixedDeltaTime * v * translateMod) + rigid.transform.right * Time.fixedDeltaTime * h * translateMod;
			transform.Rotate(new Vector3(0, rY * Time.fixedDeltaTime * rotateMod, 0));
		}


	}

	void OnCollisionEnter(Collision collision){
		var pe=collision.gameObject.GetComponentInChildren<PsychEnemy>();
		if (pe){
			health-=10;
		}
	}
}
