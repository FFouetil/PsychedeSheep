using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MazeGenerator : MonoBehaviour {

	public Transform floor;
	public Vector2 subDivs=new Vector2(10,10);
	public List<GameObject> mazeElements;
	public List<GameObject> mazeBorders;
	public List<GameObject> freeBlocks;

	public GameObject wallPrefab;
	public GameObject blockPrefab;
	public GameObject cornerPrefab;
	public GameObject spawnerPrefab;

	public bool useManualSeed=true;
	public int seed;

	// Use this for initialization
	void Start () {
		seed=Random.seed;
		//DestroyMaze();
		//DestroyBorders();
	}
	
	// Update is called once per frame
	void Update () {

		if (!Input.anyKeyDown){
			if (Random.seed != seed){

				seed=RegenSeed();

			}
		}

	}

	void OnValidate(){
		RegenSeed();
	}

	public void DestroyMaze(){
		for (int i=0; i<mazeElements.Count; ++i){
			
			DestroyImmediate(mazeElements[i]);
			//mazeElements=null;

		}
		mazeElements=new List<GameObject>((int)((subDivs.x+1)*(subDivs.y+1)));
		DestroyFreeBlocks();
	}

	public void DestroyBorders(){
		for (int i=0; i<mazeBorders.Count; ++i){

			DestroyImmediate(mazeBorders[i]);
			//mazeElements=null;

		}
		mazeBorders=new List<GameObject>((int)(subDivs.x*2+subDivs.y*2));
	}

	public void DestroyFreeBlocks(){
		for (int i=0; i<freeBlocks.Count; ++i){

			DestroyImmediate(freeBlocks[i]);
			//mazeElements=null;

		}
		freeBlocks=new List<GameObject>((int)((subDivs.x+1)*(subDivs.y+1))/2);
	}

	public int RegenSeed(){
		if (useManualSeed){
			return Random.seed=seed;
		}
		else {
			Random.seed=(int)((System.DateTime.Now.Ticks%int.MaxValue));
			return seed=Random.seed=Random.Range(int.MinValue,int.MaxValue);
		}

	}

	public void Generate(){

		DestroyMaze();

		Random.seed=seed;

		var flrScale=floor.localScale;
		Vector3 posOffsetZ=new Vector3(0,0,0.5f-flrScale.z/2);
		Vector3 posOffsetX=new Vector3(0.5f-flrScale.z/2,0,0);
		Vector3 posOffsetY=new Vector3(0,0.5f*wallPrefab.transform.localScale.y,0);
		Vector3 posOffset=new Vector3(0f,0.5f,0.5f); posOffset.Scale(flrScale);
		Vector3 pivotOffset=new Vector3(0f,0.5f,0.5f);	pivotOffset.Scale(flrScale);

		int maxBlocksX=(int)(subDivs.x*0.33f);
		int maxBlocksZ=(int)(subDivs.y*0.33f);
		int[] cntX= new int[(int)subDivs.x];
		int[] cntZ= new int[(int)subDivs.y];

		int safeMargin=2;
		for (int z=0; z<subDivs.y; z++){

			for (int x=safeMargin-2; x<subDivs.x-safeMargin+2; ++x){
				GameObject nObject=null;
				bool keepEmpty= (z<4 || z>subDivs.y-4)
					&& ( x < subDivs.x/2+4 && x > subDivs.x/2-4);
				bool placeWall=Random.value > 0.7f && cntX[x] < maxBlocksX && cntZ[z] < maxBlocksZ;

				var pos=posOffsetX+posOffsetZ+posOffsetY;
				pos.Scale(flrScale/2); pos.y*=2;

				if (placeWall && !keepEmpty){
					nObject=(GameObject)Instantiate(blockPrefab,pos,Quaternion.identity);
					mazeElements.Add(nObject);
					cntX[x]++;
					cntZ[z]++;
				}
				else{ //mark as freeblock for spawner pass
					nObject=(GameObject)Instantiate(spawnerPrefab,pos,Quaternion.identity);
					nObject.SetActive(false);
					freeBlocks.Add(nObject);
				}


				if (nObject){
					nObject.transform.SetParent(this.floor);
					nObject.transform.position+=new Vector3(x*flrScale.x/2,0,z*flrScale.z/2);
				}
					

				
				//freeBlocks

			}
		}

		int requiredSpawnerCount=16;
		int enabledSpawners=0;
		var oldseed=seed;
		RegenSeed();
		while ( enabledSpawners < requiredSpawnerCount ){
			foreach (GameObject sp in freeBlocks)
				if (Random.value < 0.050 && !sp.activeSelf){
					sp.SetActive(true); enabledSpawners++;
				}
				
		}
		//destroy and remove useless spawners
		for (int i=0; i<freeBlocks.Count;++i)
			if (!freeBlocks[i].activeSelf){
				var v=freeBlocks[i];
				if (freeBlocks.Remove(v));
					DestroyImmediate(v);
			}
		seed=oldseed;
		//freeBlocks.Capacity=freeBlocks.Count;
		//freeBlocks.RemoveAll( go => !go.activeSelf );

	}

	public void GenerateBorders(){

		DestroyBorders();
		var flrScale=floor.localScale;

		Vector3 pos1,pos2;
		Vector3 posOffsetZ=new Vector3(0,0,0.5f-flrScale.z/2);
		Vector3 posOffsetX=new Vector3(0.5f-flrScale.z/2,0,0);
		Vector3 posOffsetY=new Vector3(0,0.5f*wallPrefab.transform.localScale.y,0);
		GameObject nWall=null;

		//yes, this is an ugly couple of loops, but I'm in a rush
		for (int z=0; z<subDivs.y; z++){
			pos1=posOffsetZ+posOffsetX+posOffsetY+new Vector3(-1,0,z);
			pos2=posOffsetZ+posOffsetX+posOffsetY+new Vector3((subDivs.x),0,z);
			pos1.Scale(floor.localScale/2); pos1.y*=2;
			pos2.Scale(floor.localScale/2); pos2.y*=2;
			nWall=(GameObject)Instantiate(wallPrefab,pos1,Quaternion.identity);
			nWall.transform.SetParent(this.floor);
			mazeBorders.Add(nWall);
			nWall=(GameObject)Instantiate(wallPrefab,pos2,Quaternion.identity);
			nWall.transform.SetParent(this.floor);
			mazeBorders.Add(nWall);
		}
		for (int x=0; x<subDivs.x; x++){
			pos1=posOffsetZ+posOffsetX+posOffsetY+new Vector3(x,0,-1);
			pos2=posOffsetZ+posOffsetX+posOffsetY+new Vector3(x,0,(subDivs.y));
			pos1.Scale(floor.localScale/2); pos1.y*=2;
			pos2.Scale(floor.localScale/2); pos2.y*=2;
			nWall=(GameObject)Instantiate(wallPrefab,pos1,Quaternion.identity);
			nWall.transform.RotateAround(nWall.transform.position,Vector3.up,-90);
			nWall.transform.SetParent(this.floor);
			mazeBorders.Add(nWall);
			nWall=(GameObject)Instantiate(wallPrefab,pos2,Quaternion.identity);
			nWall.transform.RotateAround(nWall.transform.position,Vector3.up,-90);
			nWall.transform.SetParent(this.floor);
			mazeBorders.Add(nWall);
		}

	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor{

	public override void OnInspectorGUI()
	{
		

		MazeGenerator mazeGen = (MazeGenerator)target;
		if(GUILayout.Button("Generate Maze"))
		{
			var oldSeed=Random.seed;
			var newSeed=mazeGen.RegenSeed();
			if (  oldSeed != newSeed ){
				mazeGen.Generate();
			}
		
		}

		if(GUILayout.Button("Generate Borders"))
		{
			
			mazeGen.GenerateBorders();
		}

		if(GUILayout.Button("Generate All"))
		{
			mazeGen.GenerateBorders();
			mazeGen.Generate();

		}

		if(mazeGen.mazeElements.Count>0 && GUILayout.Button("Clear Maze"))
		{
			mazeGen.DestroyMaze();
			mazeGen.DestroyFreeBlocks();
		}

		if(mazeGen.mazeBorders.Count>0 && GUILayout.Button("Clear Borders"))
		{
			mazeGen.DestroyBorders();
		}

		if(mazeGen.freeBlocks.Count>0 && GUILayout.Button("Clear Free Blocks/Spawners"))
		{
			mazeGen.DestroyFreeBlocks();
		}

		DrawDefaultInspector();
	}
}
#endif