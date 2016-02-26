using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectController : MonoBehaviour {
    protected static List<EffectController> fxControllers = new List<EffectController>(8);

    [SerializeField]
    public List<ParticleSystem> partSystems;

    public bool updateLinkedControllers=true;

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

    [Range(0, 10f)]
    public float intensityModifier = 1f;



    // Use this for initialization
    void Start () {
        if (!targetEntity)
            targetEntity = GameObject.FindObjectOfType<Sheep>().transform;

		if (!scaleMorpher)
			scaleMorpher = GetComponent<ScaleMorpher>();

		if (!colorCycler)
			colorCycler = GetComponent<ColorCycler>();

        fxControllers.Add(this);
        GetComponentsInChildren<ParticleSystem>(partSystems);

    }
	
	// Update is called once per frame
	void Update () {

        //scales bounce duration according to distance
        var dist = Vector3.Distance(this.transform.position, targetEntity.position);
        var distRatio = MathEx.ValueByDeltaToLinearRatio(nearEffectDistance, farEffectDistance, dist);
        //var distRatio = 
        scaleMorpher.morphCycleDuration = Mathf.Lerp(1f/nearEffectIntensity,1f/farEffectIntensity, distRatio) * effectDistanceModifier / intensityModifier;

        foreach (ParticleSystem ps in partSystems)
        {
            var color = ps.startColor;
            color = colorCycler.CurrentColor* intensityModifier;
            color.a +=0.5f;
            ps.startColor = color;
        }

        colorCycler.effectModifier = intensityModifier;

    }


    void OnValidate()
    {
        if (updateLinkedControllers)
        {
            Debug.Log("Syncing EffectControllers");
            foreach (EffectController fxc in fxControllers)
            {
                fxc.nearEffectIntensity = nearEffectIntensity;
                fxc.farEffectIntensity = farEffectIntensity;
                fxc.nearEffectDistance = nearEffectDistance;
                fxc.farEffectDistance = farEffectDistance;
                fxc.effectDistanceModifier = effectDistanceModifier;
            }
        }

    }
    
}
