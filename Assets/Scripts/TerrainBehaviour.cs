using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private IAController AIController;
	private Vector2 posPlayer;
	private Vector2 posIA;
	private Vector2 posMeteor;
	private Vector2 frontPlayer;
	private Vector2 frontAI;
	private float rand;
	private MeteorBehavior[] meteors;
	private float[,] originalHeights;
	private float spawnCD;
	private float spawnTime;
	public GameObject prefab;
	
	void Start () {
		playerController = player.GetComponent<PlayerController>();
		AIController = IA.GetComponent<IAController>();
		hmWidth = terrain.terrainData.heightmapWidth;
		hmHeight = terrain.terrainData.heightmapHeight;

		restartHeights();

		spawnCD = Time.time;
		spawnTime = 10;
	}
	
	void Update () {
		updateSpwanTime();

		spawnMeteor();

		// On récupère les différentes données nécessaires à la boucle d'Update
		getData();

		// On récupère le point devant le joueur dans le repère du terrain
		getFrontPlayer();

		// On récupère le point devant l'IA dans le repère du terrain
		getFrontAI();
		
		// On met à jour la hauteur du terrain
		updateMapHeights();
	}

	void restartHeights() {
		float[,] heights = terrain.terrainData.GetHeights(0 , 0, hmWidth, hmHeight);

		for (int i=0; i < hmWidth; i++){
			for (int j=0; j < hmHeight; j++){
				heights[i,j] = 0;
			}
		}
		terrain.terrainData.SetHeightsDelayLOD(0, 0, heights);
	}

	void updateSpwanTime(){
		if (Time.time > 30)
			spawnTime = 5;
		if (Time.time > 60)
			spawnTime = 2;
	}

	void spawnMeteor(){
		if (spawnCD < Time.time){
			spawnCD = Time.time + spawnTime;
			int x = Random.Range(5,96);
			int z = Random.Range(5,96);
			Vector3 randomPos = new Vector3(x, 30, z);
			GameObject clone = Instantiate(prefab, randomPos, new Quaternion());
			Destroy(clone, 15);
		}
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

	void getFrontAI() {
		// On récupère la direction dans laquelle va l'IA
		direction = AIController.getDirection();

		// Et on calcule le devant de l'IA en fonction de cette direction
		frontAI.x =  IA.transform.position.x + direction.x * (radiusIA + 1);
		frontAI.y = IA.transform.position.z + direction.z * (radiusIA + 1);

		// Que l'on reconverti en coordonnées dans le repère du terrain
		frontAI = toTerrainPos(new Vector3(frontAI.x, 0, frontAI.y));
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

	Vector3 fromTerrainPos(Vector3 pos) {

		pos.x = (float)pos.x / hmWidth;
		pos.y = (float)pos.y / hmHeight;

		Vector3 coord = new Vector3();
		coord.x = pos.x * terrain.terrainData.size.x;
		coord.z = pos.y * terrain.terrainData.size.z;
		return coord;
	}

	void updateMapHeights() {
		// get the heights of the terrain under this game object
		float[,] heights = terrain.terrainData.GetHeights(0 , 0, hmWidth, hmHeight);

		rand = 1f;
		// we set each sample of the terrain in the size of the desired height
		for (int i=0; i < hmWidth; i++){
			for (int j=0; j < hmHeight; j++){
				rand = Random.Range(1f,10f);
				if (i < 5 || i > 250 || j < 5 || j > 250)
					heights[i,j] = 0;
				else
					heights[i,j] += Time.deltaTime / 50 / rand;

				// Si la position devant nous est celle de devant le joueur on grossit en fonction de la hauteur de neige
				if (j == frontPlayer.x && i == frontPlayer.y) {
					if (!player.GetComponent<PlayerController>().isStun())
						player.transform.localScale += new Vector3(1,1,1) * heights[i,j] / 30;	
					Vector3 position = new Vector3(player.transform.position.x, player.transform.localScale.y / 2, player.transform.position.z);
					player.transform.position = position;
				}

				// Si la position devant nous est celle de devant l'IA on grossit en fonction de la hauteur de neige
				if (j == frontAI.x && i == frontAI.y) {
					if (!IA.GetComponent<IAController>().isStun())
						IA.transform.localScale += new Vector3(1,1,1) * heights[i,j] / 30;
					Vector3 position = new Vector3(IA.transform.position.x, IA.transform.localScale.y / 2, IA.transform.position.z);
					IA.transform.position = position;
				}

				// On diminiue la hauteur de neige en dessous de la boule du joueur
				if (Mathf.Pow(j-posPlayer.x,2) + Mathf.Pow(i-posPlayer.y,2) < Mathf.Pow(radiusPlayer/100*257,2)) {
					heights[i,j] = heights[i,j] / 2; 
				}

				// On diminiue la hauteur de neige en dessous de la boule de l'IA
				if (Mathf.Pow(j-posIA.x,2) + Mathf.Pow(i-posIA.y,2) < Mathf.Pow(radiusIA/100*257,2)) {
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

	public  Vector3 getMaxHeightPos(){
		Vector3 res = new Vector3();
		posIA = toTerrainPos(IA.transform.position);
		float[,] heights = terrain.terrainData.GetHeights(0 , 0, hmWidth, hmHeight);
		float maxHeight = 0;
		for (int i=0; i < hmWidth; i++){
			for (int j=0; j < hmHeight; j++){
				if (Mathf.Pow(j-posIA.x,2) + Mathf.Pow(i-posIA.y,2) < Mathf.Pow(50,2) || (Mathf.Pow(j-posIA.x,2) + Mathf.Pow(i-posIA.y,2) > Mathf.Pow(20,2))) {
					if (heights[i,j] > maxHeight){
						maxHeight = heights[i,j];
						res.x = j;
						res.y = i;
					}
				}
			}
		}
		res = fromTerrainPos(res);
		return res;
	}
}