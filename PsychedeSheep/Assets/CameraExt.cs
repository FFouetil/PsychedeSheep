using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraExt : MonoBehaviour {

	public Camera cam;
	public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.LookAt(target);
	}
}
