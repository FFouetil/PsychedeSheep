using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraExt : MonoBehaviour {

	public Camera cam;
	public Transform target;

    public Material fxMaterial;
    public bool enablePostProcess=true;
    public bool autoAdjustClippingPlanes;
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

        if (autoAdjustClippingPlanes)
        {
            var oldNear = cam.nearClipPlane;
            var oldFar = cam.farClipPlane;
            var camTargetDist = Vector3.Distance(this.transform.position, target.position);
            cam.farClipPlane = camTargetDist*5;
            cam.nearClipPlane= camTargetDist*0.333f;
            Graphics.Blit(source, destination, fxMaterial);
            //cam.farClipPlane = oldFar;
            //cam.nearClipPlane = oldNear;

        }
        else
        {
            Graphics.Blit(source, destination, fxMaterial);
        }
        //fxMaterial.SetFloat("_bwBlend", intensity);
        
    }
}
