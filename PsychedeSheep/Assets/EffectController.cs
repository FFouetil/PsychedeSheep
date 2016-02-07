using UnityEngine;
using System.Collections;


public class EffectController : MonoBehaviour {

    public ScaleMorpher scaleMorpher;
    public ColorCycler colorCycler;

    public Transform targetEntity;
    [Range(0, 100f)]
    public float nearEffectIntensity = 1f;
    [Range(0, 100f)]
    public float farEffectIntensity = 10f;

    public float nearEffectDistance=0.25f;
    public float farEffectDistance = 10f;
    [Range(0,100f)]
    public float effectDistanceModifier = 1f;
    
   // public Tuple vc;

    // Use this for initialization
    void Start () {
        if (!targetEntity)
            targetEntity = GameObject.FindObjectOfType<Sheep>().transform;

	}
	
	// Update is called once per frame
	void Update () {

        //scales bounce duration according to distance
        var dist = Vector3.Distance(this.transform.position, targetEntity.position);
        var distRatio = MathHelper.DeltaValueToLinearRatio(nearEffectDistance, farEffectDistance, dist);
        //var distRatio = 
        scaleMorpher.morphCycleDuration = Mathf.Lerp(1/nearEffectIntensity,1/farEffectIntensity, distRatio) * effectDistanceModifier;

	}
}
