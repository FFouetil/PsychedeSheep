using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[DisallowMultipleComponent]
public class ScaleMorpher : MonoBehaviour, ITimed {


	public enum MorphingMode{
		Phase,
		NormalBounce,
		ReverseBounce,
		//Linear,
		//SmoothedLinear,
	}

    public float TimerDuration { get { return morphCycleDuration; } }

    public MorphingMode morphingMode;
    public float morphCycleDuration =1f;
    protected float morphCycleTimer;
	[Range(-0.95f,0.95f)]
    public float morphHorizontalRatio = 0.1f;
	[Range(-0.95f, 0.95f)]
    public float morphVerticalRatio = -0.2f;
    [Range(0.1f, 10f)]
    public float globalScaleModifier = 1f;

    Vector3 originalScale;
	float sign=1;
    float timerPhase;

	public bool IsStartingPhase { get; protected set;}
	// Use this for initialization
	void Start () {
        originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		morphCycleDuration=Mathf.Max(0.25f,morphCycleDuration);
        morphCycleTimer += Time.deltaTime;
		IsStartingPhase=!(morphCycleTimer < morphCycleDuration);
		morphCycleTimer = !IsStartingPhase ? morphCycleTimer : 0;
        
		timerPhase = GetPhaseRatio(morphingMode);

        transform.localScale = new Vector3(
            originalScale.x+ originalScale.x*timerPhase * morphHorizontalRatio,
            originalScale.y+ originalScale.y*timerPhase * morphVerticalRatio,
            originalScale.z + originalScale.z*timerPhase * morphHorizontalRatio)*globalScaleModifier; 

    }

	float GetPhaseRatio(MorphingMode mode){
		float timerRatio = morphCycleTimer / morphCycleDuration;

		switch (mode){
		case MorphingMode.Phase:return MathEx.PhaseNegPos(timerRatio);
		case MorphingMode.NormalBounce:return MathEx.BounceNegative(timerRatio);
		case MorphingMode.ReverseBounce:return MathEx.BouncePositive(timerRatio);
		//case MorphingMode.Linear:return Mathf.Lerp((sign=Mathf.Sign(0.999f-timerRatio)),-sign,timerRatio);
		//case MorphingMode.SmoothedLinear:return Mathf.SmoothStep(0*sign,1*sign,timerRatio);
		default: return 1;
		}

	}
}