using UnityEngine;
using System.Collections;

public class Sheep : MonoBehaviour {

	public ConstantForce JetPack;
	protected Rigidbody rigid;
	public float MaxJetForce=500;
	public float MaxEffectiveHeight=100;
	[Range(0,1)]
	public float FullPowerHeightRatio=0.875f;
	public float TimeToFullspeed=5;
	protected float pressDuration;
	protected float lastRefForce;
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
		if ( Input.GetButtonDown("Fly")){

			//rigid.AddForce(0,300,0);
			lastRefForce=JetPack.relativeForce.y;
		}
		else if ( Input.GetButton("Fly")){
			//JetPack
			pressDuration+=Time.fixedDeltaTime;
			RaycastHit hitFloor;
			if (Physics.Raycast(transform.position,Vector3.down, out hitFloor)){
				var heightRatio=Mathf.Clamp01((hitFloor.distance-MaxEffectiveHeight*FullPowerHeightRatio)/MaxEffectiveHeight);
				var relForce=JetPack.relativeForce; 
				lastRefForce=relForce.y=Mathf.SmoothStep(
					lastRefForce,MaxJetForce*Mathf.Log10(1+(1f-heightRatio)*9f),(pressDuration/TimeToFullspeed));
				JetPack.relativeForce=relForce;
			}


			/*
			var relForce=JetPack.relativeForce; 
			lastRefForce=relForce.y=10+Mathf.Lerp(lastRefForce,MaxJetForce,pressDuration/TimeToFullspeed);
			JetPack.relativeForce=relForce;
			*/
		}
		else if ( Input.GetButtonUp("Fly"))
		{
			pressDuration=Mathf.Max(0,pressDuration+(Time.deltaTime*2));
			var relForce=JetPack.relativeForce;
			relForce.y=lastRefForce=Mathf.Lerp(relForce.y-0.1f,0,3f);
			JetPack.relativeForce=relForce;
		}
	}
}
