using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleFxManager : MonoBehaviour {

    public List<ParticleSystem> particleSystems;
	// Use this for initialization
	void Start () {
	
	}

    public void SetEnabled(bool enabled)
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (enabled)
                ps.Play();
            else
                ps.Stop();
        }
    }

}
