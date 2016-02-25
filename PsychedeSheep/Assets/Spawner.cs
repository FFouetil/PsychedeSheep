using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public enum EnemyType
	{
		Random=0,
		Cube=1,
		Sphere=2
	}

	public EnemyType enemyType;

	public GameObject cubePrefab;
	public GameObject spherePrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn(){
	}
}
