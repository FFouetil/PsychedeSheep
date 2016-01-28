using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class ScaleBounce : MonoBehaviour {

    public float morphCycleDuration =1f;
    protected float morphCycleTimer;
    public float morphHorizontalRatio = 1f;
    public float morphVerticalRatio = 1f;
    Vector3 originalScale;


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
        var timerRatio = morphCycleTimer / morphCycleDuration;
        timerPhase = Mathf.Cos(timerRatio * Mathf.PI*2f);

        transform.localScale = new Vector3(originalScale.x+timerPhase* morphHorizontalRatio, originalScale.y- timerPhase * morphVerticalRatio, originalScale.z + timerPhase* morphHorizontalRatio); 

    }
}
