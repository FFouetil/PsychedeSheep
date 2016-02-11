using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraExt : MonoBehaviour {

	public Camera cam;
	public Transform target;

    public Material fxMaterial;
    public bool enablePostProcess=true;
    public bool autoAdjustClippingPlanes;


    private float initHeightAtDist;
    public bool dzEnabled;


    // Use this for initialization
    void Start () {
        cam.depthTextureMode = DepthTextureMode.Depth;
        //StartDZ();
    }
	
	// Update is called once per frame
	void Update () {
        if (target)
		    cam.transform.LookAt(target);

        if (dzEnabled)
        {
            // Measure the new distance and readjust the FOV accordingly.
            var currDistance = Vector3.Distance(transform.position, target.position);
            cam.fieldOfView = FOVForHeightAndDistance(initHeightAtDist, currDistance);
            // Simple control to allow the camera to be moved in and out using the up/down arrows.
            transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * Time.deltaTime * 20f);
        }


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




    // Calculate the frustum height at a given distance from the camera.
    float FrustumHeightAtDistance(float distance)
    {
        return 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    // Calculate the FOV needed to get a given frustum height at a given distance.
    float FOVForHeightAndDistance(float height, float distance)
    {
        return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }

    // Start the dolly zoom effect.
    void StartDZ()
    {
        var distance = Vector3.Distance(transform.position, target.position);
        initHeightAtDist = FrustumHeightAtDistance(distance);
        dzEnabled = true;
    }

    // Turn dolly zoom off.
    void StopDZ()
    {
        dzEnabled = false;
    }

}
