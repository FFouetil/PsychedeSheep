using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class ScaleBounce : MonoBehaviour {

	public enum MorphingMode{
		Phase,
		NormalBounce,
		ReverseBounce,
		//Linear,
		//SmoothedLinear,
	}

	public MorphingMode morphingMode;
    public float morphCycleDuration =1f;
    protected float morphCycleTimer;
	[Range(-0.95f,0.95f)]
    public float morphHorizontalRatio = 0.1f;
	[Range(-0.95f, 0.95f)]
    public float morphVerticalRatio = -0.2f;
    Vector3 originalScale;
	float sign=1;


    //[SerializeField]
    float timerPhase;
	// Use this for initialization
	void Start () {
        originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        morphCycleTimer += Time.deltaTime;
        morphCycleTimer = morphCycleTimer < morphCycleDuration ? morphCycleTimer : 0;
        
		timerPhase = GetPhaseRatio(morphingMode);

        transform.localScale = new Vector3(
            originalScale.x+ originalScale.x*timerPhase * morphHorizontalRatio,
            originalScale.y+ originalScale.y*timerPhase * morphVerticalRatio,
            originalScale.z + originalScale.z*timerPhase * morphHorizontalRatio); 

    }

	float GetPhaseRatio(MorphingMode mode){
		var timerRatio = morphCycleTimer / morphCycleDuration;

		switch (mode){
		case MorphingMode.Phase:return Mathf.Sin(timerRatio * Mathf.PI*2f);
		case MorphingMode.NormalBounce:return Mathf.Sin(timerRatio * Mathf.PI+Mathf.PI);
		case MorphingMode.ReverseBounce:return Mathf.Sin(timerRatio * Mathf.PI);
		//case MorphingMode.Linear:return Mathf.Lerp((sign=Mathf.Sign(0.999f-timerRatio)),-sign,timerRatio);
		//case MorphingMode.SmoothedLinear:return Mathf.SmoothStep(0*sign,1*sign,timerRatio);
		default: return 1;
		}

	}
}