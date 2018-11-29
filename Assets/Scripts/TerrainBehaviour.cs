using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBehaviour : MonoBehaviour {
	
	public Terrain terrain; // terrain to modify
	public GameObject player;
	public GameObject IA;
	private int hmWidth; // heightmap width
	private int hmHeight; // heightmap height
	private float radiusPlayer;
	private float radiusIA;
	private Vector3 direction;
	private PlayerController playerController;
	private Vector2 posPlayer;
	private Vector2 posIA;
	private Vector2 posMeteor;
	private Vector2 frontPlayer;
	private int rand;
	private float radiusMeteor;
	private GameObject[] meteors;
	private float[,] originalHeights;
	
	void Start () {
		playerController = player.GetComponent<PlayerController>();
		hmWidth = terrain.terrainData.heightmapWidth;
		hmHeight = terrain.terrainData.heightmapHeight;
		originalHeights = terrain.terrainData.GetHeights(0, 0, hmWidth, hmHeight);

		radiusMeteor = 2.0f;
	
	}
	
	void Update () {
		meteors = GameObject.FindGameObjectsWithTag("Meteor");
		direction = playerController.getDirection();
		radiusPlayer = player.transform.localScale.x / 2;
		radiusIA = IA.transform.localScale.x / 2;
		posPlayer = toTerrainPos(player.transform.position);
		posIA = toTerrainPos(IA.transform.position);

		frontPlayer.x =  player.transform.position.x + direction.x * (radiusPlayer + 1);
		frontPlayer.y = player.transform.position.z + direction.z * (radiusPlayer + 1);
		frontPlayer = toTerrainPos(new Vector3(frontPlayer.x, 0, frontPlayer.y));
	
		setMapHeights();
	}

	Vector3 toTerrainPos(Vector3 pos){
		// get the normalized position of the player relative to the terrain
		Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
		Vector3 coord;
		coord.x = tempCoord.x / terrain.terrainData.size.x;
		coord.y = tempCoord.y / terrain.terrainData.size.y;
		coord.z = tempCoord.z / terrain.terrainData.size.z;
	
		// get the position of the terrain heightmap where this game object is
		pos.x = (int) (coord.x * hmWidth);
		pos.y = (int) (coord.z * hmHeight);
		return pos;
	}

	void setMapHeights(){
		// get the heights of the terrain under this game object
		float[,] heights = terrain.terrainData.GetHeights(0 , 0, hmWidth, hmHeight);

		rand = 1;
		// we set each sample of the terrain in the size to the desired height
		for (int i=0; i < hmWidth; i++){
			for (int j=0; j < hmHeight; j++){
				rand = Random.Range(1,10);
				heights[i,j] += Time.deltaTime / 50 / rand;
				if (j == frontPlayer.x && i == frontPlayer.y){
						player.transform.localScale += new Vector3(1,1,1) * heights[i,j] / 30 ;
						Vector3 position = new Vector3(player.transform.position.x, player.transform.localScale.y / 2, player.transform.position.z);
						player.transform.position = position;
					}
				if (Mathf.Pow(j-posPlayer.x,2) + Mathf.Pow(i-posPlayer.y,2) < Mathf.Pow(radiusPlayer/100*257,2)){
					heights[i,j] = heights[i,j] / 2; 
				}
				if (Mathf.Pow(j-posIA.x,2) + Mathf.Pow(i-posIA.y,2) < Mathf.Pow(radiusIA/100*257,2)){
					if (j == posIA.x && i == posIA.y)
						IA.transform.localScale += new Vector3(1,1,1) * heights[i,j] / 30;
					heights[i,j] = heights[i,j] / 2;	 
				}
			}
		}
		for (int i=0; i<meteors.Length; i++)
			meteorBehaviour(meteors[i], ref heights);
		// set the new height
		terrain.terrainData.SetHeights(0, 0, heights);
	}

	void meteorBehaviour(GameObject meteor, ref float[,] heights){
		posMeteor = toTerrainPos(meteor.transform.position);
		float radius = Mathf.Pow(radiusMeteor/100*257,2);
		for (int i=0; i < hmWidth; i++){
			for (int j=0; j < hmHeight; j++){
				if (Mathf.Pow(j-posMeteor.x,2) + Mathf.Pow(i-posMeteor.y,2) < radius)
					heights[i,j] -= Time.deltaTime / 25 ;
				if (heights[i,j] < 0)
					heights[i,j] = 0;
			}
		}
		if (Mathf.Pow(posPlayer.x-posMeteor.x,2) + Mathf.Pow(posPlayer.y-posMeteor.y,2) < radius + 50)
			player.transform.localScale -= new Vector3(1,1,1) * 0.02f;
	}

	void OnDestroy()
         {
             terrain.terrainData.SetHeights(0, 0, originalHeights);
         }
}
