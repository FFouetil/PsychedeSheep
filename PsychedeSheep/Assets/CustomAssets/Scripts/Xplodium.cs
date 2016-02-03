using UnityEngine;
using System.Collections;

public class Xplodium : MonoBehaviour {

    public Transform explosionFxPrefab;
    [Range(0,10000)]
    public float explosionForce=1000f;
    [Range(0.1f, 10f)]
    public float explosionSize=1f;

    protected bool hasExploded = false;
    
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter()
    {
        Explode();
    }

    public void Explode(bool forceExplode=false)
    {
        if ( forceExplode || !hasExploded)
        {
            hasExploded = true;
        }
        
    }
}
