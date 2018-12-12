using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public GameObject ia;
	public GameObject player;
	private float goalScale;

	// Use this for initialization
	void Awake () {
		goalScale = 12;
	}
	
	// Update is called once per frame
	void Update () {
		if (ia.transform.localScale.x > goalScale )
			gameOver();
		else if (player.transform.localScale.x > goalScale)
			win();
	}

	public void gameOver(){
		SceneManager.LoadScene("GameOverScene");
	}

	public void win(){
		SceneManager.LoadScene("WinScene");
	}

	public float getGoalScale() {
		return goalScale;
	}
}
