using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	private GameObject ia;
	private GameObject player;
	private float goalScale;
	private bool difficult;

	// Use this for initialization
	void Awake () {
		goalScale = 12;
		ia = GameObject.Find("IA");
		player = GameObject.Find("Player");
		difficult = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (ia != null) {
			if (ia.transform.localScale.x > goalScale )
				gameOver();
			else if (player.transform.localScale.x > goalScale)
				win();
		}
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

	public bool isDifficultOn() {
		return difficult;
	}
}
