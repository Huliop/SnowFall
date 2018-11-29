using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehavior : MonoBehaviour {

	public List<Vector2> listPos;

	public Terrain terrain;
	int hmHeight;
	int hmWidth;
	float radiusMeteor = 4f;

	void Start () {
		terrain = FindObjectOfType<Terrain>();

		listPos = new List<Vector2>();

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
}
