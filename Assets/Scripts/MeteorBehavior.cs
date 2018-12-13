using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehavior : MonoBehaviour {

	public List<Vector2> listPos;

	public Terrain terrain;
	private Rigidbody rb;
	bool done;
	int hmHeight;
	int hmWidth;
	float radiusMeteor = 4f;
	private GameObject player;
	private bool collidingPlayer;

	void Awake() {
		terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
	}

	void Start () {

		listPos = new List<Vector2>();
		done = false;
		rb = GetComponent<Rigidbody>();
		hmWidth = terrain.terrainData.heightmapWidth;
		hmHeight = terrain.terrainData.heightmapHeight;

		Vector2 posMeteor = toTerrainPos(transform.position);
		float radius = Mathf.Pow(radiusMeteor/100*257,2);
		for (int i=0; i < hmWidth; i++) {
			for (int j=0; j < hmHeight; j++) {
				if (Mathf.Pow(j-posMeteor.x,2) + Mathf.Pow(i-posMeteor.y,2) < radius)
					listPos.Add((new Vector2(i, j)));
			}
		}
	}

	void Update(){
		if(transform.position.y > 0){
			transform.position = transform.position - new Vector3(0,16,0) * Time.deltaTime;
		}
		if (transform.position.y < 0 && !done){
			rb.isKinematic = true;
			done = true;
		}
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

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player"){
			player = collider.gameObject;
			collidingPlayer = true;
		}
	}

		void OnTriggerExit(Collider collider){
		if (collider.gameObject.tag == "Player"){
			player = collider.gameObject;
			collidingPlayer = false;
		}
	}

	void OnDestroy(){
		if (collidingPlayer){
			if (player.GetComponent<PlayerController>().melt()){
				player.GetComponent<PlayerController>().meltOff();
			} 
			else if (player.GetComponent<IAController>().melt()){
				player.GetComponent<IAController>().meltOff();
			}
		}
	}
}
