using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public enum EnemyType
	{
		Random=0,
		Cube=1,
		Sphere=2
	}

	public EnemyType enemyType=EnemyType.Random;

	public GameObject cubePrefab;
	public GameObject spherePrefab;

	public int maxAliveSpawnees;
	public List<GameObject> aliveSpawnees;

	// Use this for initialization
	void Start () {
		aliveSpawnees=new List<GameObject>(3);
		Spawn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn(){
		GameObject e=null;
		if (enemyType == EnemyType.Random){
			e= (Random.value > 0.5f) ?
				(GameObject)Instantiate(cubePrefab,transform.position,Quaternion.identity) : (GameObject)Instantiate(spherePrefab,transform.position,Quaternion.identity);;
		}
		else if (enemyType == EnemyType.Cube){
			
			e=(GameObject)Instantiate(cubePrefab,transform.position,Quaternion.identity);
		}
		else e=(GameObject)Instantiate(spherePrefab,transform.position,Quaternion.identity);
		aliveSpawnees.Add(e);
	
	}
}
