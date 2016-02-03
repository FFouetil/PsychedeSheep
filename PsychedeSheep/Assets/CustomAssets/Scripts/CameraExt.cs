using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraExt : MonoBehaviour {

	public Camera cam;
	public Transform target;

    public Material fxMaterial;
    public bool enablePostProcess=true;
    // Use this for initialization
    void Start () {
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
	
	// Update is called once per frame
	void Update () {
        if (target)
		    cam.transform.LookAt(target);
	}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int intensity = 1;
        if (intensity == 0 || fxMaterial == null || !enablePostProcess )
        {
            Graphics.Blit(source, destination);
            return;
        }

        fxMaterial.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, fxMaterial);
    }
}
