using UnityEngine;
using System.Collections.Generic;

public class CameraIntro : MonoBehaviour {

	public Camera introCam;
	public List<Transform> path;
	int nextPathIndex=0;
	// Use this for initialization
	void Start () {
		introCam.transform.position=path[nextPathIndex].position;
		Time.timeScale=0.1f;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (nextPathIndex<path.Count-1){
			var dist=Vector3.Distance(introCam.transform.position,path[nextPathIndex+1].position);
			if (dist>0.001f){
				//transform.position= Vector3.Lerp();
				introCam.transform.position=Vector3.MoveTowards(
					introCam.transform.position,path[nextPathIndex+1].position,Time.unscaledDeltaTime*25);

			}
			else{
				nextPathIndex++;
			}
		}
		else {
			Time.timeScale=1f;
			introCam.depth=-1;
			introCam.gameObject.SetActive(false);

		}

	}
}
