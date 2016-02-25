using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
public class MazeGenerator : MonoBehaviour {

	public Transform floor;
	public Vector2 subDivs=new Vector2(10,10);
	public List<GameObject> mazeElements;
	public List<GameObject> mazeBorders;

	public GameObject wallPrefab;
	public GameObject blockPrefab;
	public GameObject cornerPrefab;

	public bool useManualSeed=true;
	public int seed=Random.seed;

	// Use this for initialization
	void Start () {
		//DestroyMaze();
		//DestroyBorders();
	}
	
	// Update is called once per frame
	void Update () {

		if (!Input.anyKeyDown){
			if (Random.seed != seed){

				Random.seed=seed;

			}
		}

	}

	void OnValidate(){
		if (useManualSeed){
			Random.seed=seed;
		}
		else {
			Random.seed=(int)System.DateTime.Now.ToFileTimeUtc();
		}
	}

	public void DestroyMaze(){
		for (int i=0; i<mazeElements.Count; ++i){
			
			DestroyImmediate(mazeElements[i]);
			//mazeElements=null;

		}
		mazeElements=new List<GameObject>((int)((subDivs.x+1)*(subDivs.y+1)));
	}

	public void DestroyBorders(){
		for (int i=0; i<mazeBorders.Count; ++i){

			DestroyImmediate(mazeBorders[i]);
			//mazeElements=null;

		}
		mazeBorders=new List<GameObject>((int)(subDivs.x*2+subDivs.y*2));
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

		int maxBlocksX=(int)(subDivs.x*0.4f);
		int maxBlocksZ=(int)(subDivs.y*0.4f);
		int[] cntX= new int[(int)subDivs.x];
		int[] cntZ= new int[(int)subDivs.y];

		int safeMargin=2;
		for (int z=safeMargin+2; z<subDivs.y-safeMargin; z++){

			for (int x=safeMargin-1; x<subDivs.x-safeMargin+1; ++x){

				bool spawn=Random.value > 0.15f && cntX[x] < maxBlocksX && cntZ[z] < maxBlocksZ;
				if (spawn){
					var pos=posOffsetX+posOffsetZ+posOffsetY;
					pos.Scale(flrScale/2); pos.y*=2;

					GameObject nWall=(GameObject)Instantiate(blockPrefab,pos,Quaternion.identity);

					nWall.transform.SetParent(this.floor);
					nWall.transform.position+=new Vector3(x*flrScale.x/2,0,z*flrScale.z/2);

					mazeElements.Add(nWall);
					cntX[x]++;
					cntZ[z]++;
				}


			}
		}

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
		for (int z=1; z<subDivs.y-1; z++){
			pos1=posOffsetZ+posOffsetX+posOffsetY+new Vector3(0,0,z);
			pos2=posOffsetZ+posOffsetX+posOffsetY+new Vector3((subDivs.x-1),0,z);
			pos1.Scale(floor.localScale/2); pos1.y*=2;
			pos2.Scale(floor.localScale/2); pos2.y*=2;
			nWall=(GameObject)Instantiate(wallPrefab,pos1,Quaternion.identity);
			nWall.transform.SetParent(this.floor);
			mazeBorders.Add(nWall);
			nWall=(GameObject)Instantiate(wallPrefab,pos2,Quaternion.identity);
			nWall.transform.SetParent(this.floor);
			mazeBorders.Add(nWall);
		}
		for (int x=1; x<subDivs.x-1; x++){
			pos1=posOffsetZ+posOffsetX+posOffsetY+new Vector3(x,0,0);
			pos2=posOffsetZ+posOffsetX+posOffsetY+new Vector3(x,0,(subDivs.y-1));
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


[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor{

	public override void OnInspectorGUI()
	{
		

		MazeGenerator myScript = (MazeGenerator)target;
		if(GUILayout.Button("Generate Maze"))
		{
			if ( myScript.seed != Random.seed )
				Random.seed=myScript.seed;
				myScript.Generate();
		}

		if(GUILayout.Button("Generate Borders"))
		{

			myScript.GenerateBorders();
		}

		if(GUILayout.Button("Clear Maze"))
		{
			myScript.DestroyMaze();
		}

		if(GUILayout.Button("Clear Borders"))
		{
			myScript.DestroyBorders();
		}

		DrawDefaultInspector();
	}
}