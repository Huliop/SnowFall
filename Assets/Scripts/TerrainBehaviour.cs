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
	private float rand;
	private MeteorBehavior[] meteors;
	private float[,] originalHeights;
	
	void Start () {
		playerController = player.GetComponent<PlayerController>();
		hmWidth = terrain.terrainData.heightmapWidth;
		hmHeight = terrain.terrainData.heightmapHeight;
		originalHeights = terrain.terrainData.GetHeights(0, 0, hmWidth, hmHeight);
	}
	
	void Update () {

		// On récupère les différentes données nécessaires à la boucle d'Update
		getData();

		// On récupère le point devant le joueur dans le repère du terrain
		getFrontPlayer();
		
		// On met à jour la hauteur du terrain
		updateMapHeights();
	}

	void getData() {

		// On référencie les météores (A enlver plus tard)
		meteors = GameObject.FindObjectsOfType<MeteorBehavior>();

		// Les rayons du joueur et de l'IA
		radiusPlayer = player.transform.localScale.x / 2;
		radiusIA = IA.transform.localScale.x / 2;

		// Les positions du joueur et de l'IA
		posPlayer = toTerrainPos(player.transform.position);
		posIA = toTerrainPos(IA.transform.position);
	}

	void getFrontPlayer() {

		// On récupère la direction dans laquelle va le joueur
		direction = playerController.getDirection();

		// Et on calcule le devant du joueur en fonction de cette direction
		frontPlayer.x =  player.transform.position.x + direction.x * (radiusPlayer + 1);
		frontPlayer.y = player.transform.position.z + direction.z * (radiusPlayer + 1);

		// Que l'on reconverti en coordonnées dans le repère du terrain
		frontPlayer = toTerrainPos(new Vector3(frontPlayer.x, 0, frontPlayer.y));
	}

	Vector3 toTerrainPos(Vector3 pos) {

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

	void updateMapHeights() {
		// get the heights of the terrain under this game object
		float[,] heights = terrain.terrainData.GetHeights(0 , 0, hmWidth, hmHeight);

		rand = 1f;
		// we set each sample of the terrain in the size of the desired height
		for (int i=0; i < hmWidth; i++){
			for (int j=0; j < hmHeight; j++){
				rand = Random.Range(1f,10f);
				heights[i,j] += Time.deltaTime / 50 / rand;
				if (j == frontPlayer.x && i == frontPlayer.y) {
					player.transform.localScale += new Vector3(1,1,1) * heights[i,j] / 30;
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
		terrain.terrainData.SetHeightsDelayLOD(0, 0, heights);
	}

	void meteorBehaviour(MeteorBehavior meteor, ref float[,] heights) {
		
		// La neige autour du météore fond
		foreach (Vector2 pos in meteor.listPos) {
			heights[(int) pos.x, (int) pos.y] -= Time.deltaTime / 20 ;
			if (heights[(int) pos.x, (int) pos.y] < 0)
				heights[(int) pos.x, (int) pos.y] = 0;
		}
				
	}

	void OnDestroy()
		{
			terrain.terrainData.SetHeights(0, 0, originalHeights);
		}
}